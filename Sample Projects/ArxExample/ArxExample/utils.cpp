#include "utils.h"
#include "stdafx.h"
#include <list>

#define RED						1
#define BLACK_WHITE				7
#define LAYER_NAME				"Ballon_1"

namespace Utils
{
	Acad::ErrorStatus CheckLayerName(const CString& layer_name, AcDbObjectId& layer_id)
	{
		Acad::ErrorStatus error_status = Acad::eOk;
		AcDbLayerTable* layer_table = NULL;
		AcDbDatabase* db = acdbHostApplicationServices()->workingDatabase();
		error_status = db->getLayerTable((AcDbSymbolTable*&) layer_table, AcDb::kForRead);

		if(Acad::eOk != error_status)
			return error_status;

		layer_table = AcDbLayerTable::cast(layer_table);

		AcDbLayerTableIterator* layer_table_iterator;
		error_status = layer_table->newIterator(layer_table_iterator);
		layer_table->close();

		if(Acad::eOk != error_status)
			return error_status;

		const TCHAR* name;
		AcDbLayerTableRecord* layer_table_record = NULL;
		
		for(; !layer_table_iterator->done(); layer_table_iterator->step())
		{
			error_status = layer_table_iterator->getRecord(layer_table_record, AcDb::kForRead);
			if(Acad::eOk != error_status)
			{
				delete layer_table_iterator;
				return error_status;
			}

			layer_table_record->getName(name);

			if(layer_name == CString(name))
			{
				delete layer_table_iterator;
				layer_id = layer_table_record->id();
				layer_table_record->close();
				return error_status;
			}

			layer_table_record->close();
		}

		delete layer_table_iterator;
		return error_status;
	}

	AcDbLayerTableRecord* GetLayerById_ForWrite(const AcDbObjectId& layer_id)
	{
		AcDbLayerTableRecord* layer_table_record = NULL;
		AcDbDatabase* database = acdbHostApplicationServices()->workingDatabase();
		AcDbLayerTable* layer_table = NULL;
		
		Acad::ErrorStatus error_status = database->getLayerTable((AcDbSymbolTable*&)layer_table, AcDb::kForRead);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the layer table!\n"));
			return layer_table_record;
		}

		layer_table = AcDbLayerTable::cast(layer_table);

		AcDbLayerTableIterator* layer_table_iterator;
		error_status = layer_table->newIterator(layer_table_iterator);
		layer_table->close();

		if(Acad::eOk != error_status)
			return (layer_table_record = NULL);
			
		for(; !layer_table_iterator->done(); layer_table_iterator->step())
		{
			error_status = layer_table_iterator->getRecord(layer_table_record, AcDb::kForWrite);
			if(Acad::eOk != error_status)
			{
				delete layer_table_iterator;
				return (layer_table_record = NULL);
			}

			if(layer_id == layer_table_record->id())
			{
				delete layer_table_iterator;
				return layer_table_record;
			}

			layer_table_record->close();
		}

		delete layer_table_iterator;
		return (layer_table_record = NULL);
	}

	AcDbObjectId CreateNewLayer(const CString& layer_name, const int& color_index)
	{
		AcDbObjectId ret = AcDbObjectId::kNull;
		Acad::ErrorStatus error_status = CheckLayerName(layer_name, ret);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Error CreateNewLayer!\n"));
			return ret;
		}

		if(AcDbObjectId::kNull != ret)
		{
			acutPrintf(_T("Layer is exist!\n"));
			return ret;
		}

		AcDbLayerTable* layer_table = NULL;

		if(Acad::eOk == acdbCurDwg()->getLayerTable(layer_table, AcDb::kForWrite))
		{
			AcDbLayerTableRecord* layer_table_record = new AcDbLayerTableRecord();
			layer_table_record->setName(layer_name);
			AcCmColor color;
			color.setColorIndex(color_index);
			layer_table_record->setColor(color);

			layer_table->add(layer_table_record);
			ret = layer_table_record->id();
			layer_table_record->close();

			acutPrintf(_T("Created new layer name: %s\n"), layer_name);
		}
		else
		{
			acutPrintf(_T("Can not create layer name: %s\n"), layer_name);
		}

		layer_table->close();

		return ret;
	}

	Acad::ErrorStatus MakeGroup(const AcDbObjectIdArray& object_id_array, AcDbObjectId& group_id)
	{ 
		AcDbDictionary* group_dict = NULL;
		Acad::ErrorStatus error_status;

		if(Acad::eOk == (error_status = acdbCurDwg()->getGroupDictionary(group_dict, AcDb::kForWrite)))
		{
			AcDbGroup* group = new AcDbGroup();
			error_status = group_dict->setAt(_T("custom_group"), group, group_id);
			if(Acad::eOk == error_status)
			{
				for(int i = 0; i < object_id_array.length(); i++)
					group->append(object_id_array[i]);

				group->close();
				acutPrintf(_T("Group is created!\n"));
			}
			else
			{
				acutPrintf(_T("can't make a new group! Error Status: %d\n"), error_status);
				delete group;
			}

			group_dict->close();
		}

		return error_status;
	}

	Acad::ErrorStatus GetAllEntities(std::list<AcDbEntity*>& list_obj, const AcDbObjectId& layer_id)
	{
		Acad::ErrorStatus error_status;

		AcDbDatabase* database = acdbHostApplicationServices()->workingDatabase();
		AcDbBlockTable* block_table = NULL;

		if(Acad::eOk != (error_status = database->getBlockTable(block_table, AcDb::kForRead)))
		{
			acutPrintf(_T("Can not get the block table!\n"));
			return error_status;
		}

		AcDbBlockTableRecord* block_table_record = NULL;
		if(Acad::eOk == (error_status = block_table->getAt(ACDB_MODEL_SPACE, block_table_record, AcDb::kForRead)))
		{
			block_table->close();
			AcDbBlockTableRecordIterator* block_table_record_iterator = NULL;
			block_table_record->newIterator(block_table_record_iterator);
			AcDbEntity* obj;

			for(block_table_record_iterator->start();
				!block_table_record_iterator->done();
				block_table_record_iterator->step())
			{
				if(Acad::eOk == block_table_record_iterator->getEntity(obj, AcDb::kForWrite))
				{
					list_obj.push_back(obj);
				}
			}

			delete block_table_record_iterator;
		}

		block_table_record->close();

		return Acad::eOk;
	}

	static Acad::ErrorStatus AddLineType()
	{
		Acad::ErrorStatus error_status = Acad::eOk;
		const ACHAR* file_name = _T("acad.lin");
		TCHAR file_path[MAX_PATH];
		if(RTNORM != acedFindFile(file_name, file_path))
		{
			acutPrintf(_T("can't find the file!\n"));
			return error_status;
		}

		AcDbLinetypeTable* linetype_table = NULL;
		error_status = acdbCurDwg()->getLinetypeTable(linetype_table, AcDb::kForRead);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the linetype table!\n"));
			return error_status;
		}
				
		linetype_table->close();
		AcDbObjectId obj_id;

		const ACHAR* entry_name = ACRX_T("HIDDEN");
		if(Acad::eOk != (error_status = linetype_table->getAt(entry_name, obj_id))) 
		{
			error_status = acdbCurDwg()->loadLineTypeFile(entry_name, file_path);
			acutPrintf(_T("error status1: %d\n"), error_status);
		}
		else 
		{
			acutPrintf(_T("error status2: %d\n"), error_status);
		}

		entry_name = _T("CONTINUOUS");
		if(Acad::eOk != (error_status = linetype_table->getAt(entry_name, obj_id)))
		{
			acdbCurDwg()->loadLineTypeFile(entry_name, file_name);
		}

		entry_name = _T("CENTER");
		if(Acad::eOk != (error_status = linetype_table->getAt(entry_name, obj_id)))
		{
			acdbCurDwg()->loadLineTypeFile(entry_name, file_name);
		}

		return Acad::eOk;
	}

	void DrawOneBalloon(const AcGePoint3d& location, const CString& num, const AcDbObjectId& layer_id)
	{
		AcGePoint3d start_point(location);
		AcGePoint3d end_point(10.0, 10.0, 0.0);
		Acad::ErrorStatus error_status;

		end_point.x += location.x;
		end_point.y += location.y;
		end_point.z += location.z;

		AcDbLine* line = new AcDbLine(start_point, end_point);
		line->setLayer(layer_id);
		line->setLinetype(_T("CONTINUOUS"));
		line->close();

		AcGePoint3d center_point(end_point);
		AcGeVector3d vector(0.0, 5.0, 0.0);
		center_point += vector;
		acutPrintf(_T("%f, %f, %f\n"), center_point.x, center_point.y, center_point.z);
		AcGeVector3d normal(0.0, 0.0, 1.0);
		AcDbCircle* circle = new AcDbCircle(center_point, normal, 5.0);
		circle->setLayer(layer_id);
		circle->setLinetype(_T("CONTINUOUS"));
		circle->close();

		AcDbText* text = new AcDbText(center_point, num, AcDbObjectId::kNull, 3.0, 0.0);
		text->setLayer(layer_id);
		center_point.y -= text->height()/2;
		text->setPosition(center_point);
		text->close();

		AcDbDatabase* database = acdbHostApplicationServices()->workingDatabase();
		AcDbBlockTable* block_table = NULL;
		database->getBlockTable(block_table, AcDb::kForRead);

		AcDbBlockTableRecord* block_table_record = NULL;
		block_table->getAt(ACDB_MODEL_SPACE, block_table_record, AcDb::kForWrite);

		block_table->close();

		AcDbObjectId line_id = AcDbObjectId::kNull;
		block_table_record->appendAcDbEntity(line_id, line);
		line->close();
		AcDbObjectId circle_id = AcDbObjectId::kNull;
		block_table_record->appendAcDbEntity(circle_id, circle);
		circle->close();
		AcDbObjectId text_id = AcDbObjectId::kNull;
		block_table_record->appendAcDbEntity(text_id, text);
		text->close();

		block_table_record->close();
	}

	void DrawBalloons()
	{
		AcDbObjectId layer_id = AcDbObjectId::kNull;
		Acad::ErrorStatus error_status = CheckLayerName(CString(LAYER_NAME), layer_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Error DrawBallon: %d\n"), error_status);
			return;
		}

		if(AcDbObjectId::kNull == layer_id)
			layer_id = CreateNewLayer(CString(LAYER_NAME), BLACK_WHITE);

		AcDbLayerTableRecord* layer = GetLayerById_ForWrite(layer_id);
		if(NULL == layer)
		{
			acutPrintf(_T("Error with layer!\n"));
			return;
		}

		DrawOneBalloon(AcGePoint3d(0.0, 0.0, 0.0), CString("1"), layer_id);
		DrawOneBalloon(AcGePoint3d(15.0, 0.0, 0.0), CString("2"), layer_id);
		DrawOneBalloon(AcGePoint3d(30.0, 0.0, 0.0), CString("3"), layer_id);
		DrawOneBalloon(AcGePoint3d(45.0, 0.0, 0.0), CString("4"), layer_id);
		DrawOneBalloon(AcGePoint3d(60.0, 0.0, 0.0), CString("5"), layer_id);

		layer->close();
	}

	void ChangeColorBallons()
	{
		AcDbObjectId layer_id = AcDbObjectId::kNull;
		Acad::ErrorStatus error_status = CheckLayerName(CString(LAYER_NAME), layer_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Not Exist Layer: %s\n"), LAYER_NAME);
			return;
		}

		std::list<AcDbEntity*> objs;
		GetAllEntities(objs, layer_id);
		AcDbEntity* entity;

		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();

			int color_index;
			AcCmColor color;
			if(AcDbLine::desc() == entity->isA())
			{
				color_index = entity->colorIndex();
				if(1 != color_index)
					color.setColorIndex(1);
				else
					color.setColorIndex(2);
		}
			else if(AcDbCircle::desc() == entity->isA())
			{
				color_index = entity->colorIndex();
				if(4 != color_index)
					color.setColorIndex(4);
				else
					color.setColorIndex(5);
		}
			else if(AcDbText::desc() == entity->isA())
			{
				color_index = entity->colorIndex();
				if(6 != color_index)
					color.setColorIndex(6);
				else
					color.setColorIndex(3);
			}
			else 
			{
				color.setColorIndex(7);
			}

			entity->setColor(color);
			entity->close();
		}
	}

	void ChangeLineType()
	{
		static int count = 0;
		if(0 == count)
		{
			count++;
			AddLineType();
		}
			
		AcDbObjectId layer_id = AcDbObjectId::kNull;
		Acad::ErrorStatus error_status = CheckLayerName(CString(LAYER_NAME), layer_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Not Exist Layer: %s\n"), LAYER_NAME);
			return;
		}

		std::list<AcDbEntity*> objs;
		GetAllEntities(objs, layer_id);
		AcDbEntity* entity;

		AcDbLinetypeTable line_type;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();

			ACHAR* line_type = entity->linetype();
			CString line_type_name = &line_type[0];

		if(AcDbLine::desc() == entity->isA())
			{
				if(CString(_T("HIDDEN")) == line_type_name)
					((AcDbLine*)entity)->setLinetype(_T("CONTINUOUS"));
				else
					((AcDbLine*)entity)->setLinetype(_T("HIDDEN"));

			}
			else if(AcDbCircle::desc() == entity->isA())
			{
				if(CString(_T("CENTER")) == line_type_name)
					((AcDbCircle*)entity)->setLinetype(_T("CONTINUOUS"));
				else
					((AcDbCircle*)entity)->setLinetype(_T("CENTER"));
			}

			if(line_type)
				delete line_type;

			entity->close();
		}
	}

	void GroupBalloon()
	{
		AcDbObjectId group_id = AcDbObjectId::kNull;
		std::list<AcDbEntity*> objs;
		AcDbObjectId layer_id;
		Acad::ErrorStatus error_status = CheckLayerName(CString(LAYER_NAME), layer_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the layer!\n"));
			return;
		}

		error_status = GetAllEntities(objs, layer_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the entities!\n"));
			return;
		}

		AcDbObjectIdArray objs_id_arr;
		AcDbEntity* entity = NULL;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();

			objs_id_arr.append(entity->id());
			entity->close();
		}

		error_status = MakeGroup(objs_id_arr, group_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't make group!\n"));
			return;
		}

		acutPrintf(_T("Make group complete!\n"));
	}

	void MoveHozirontal(const bool& is_move_right)
	{
		Acad::ErrorStatus error_status;
		AcDbObjectId layer_id;
		error_status = CheckLayerName(CString(LAYER_NAME), layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the layer!\n"));
			return;
		}

		std::list<AcDbEntity*> objs;
		error_status = GetAllEntities(objs, layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the entities in the layer!\n"));
			return;
		}

		double lenght_move = -1.0;
		for(auto ii = objs.begin(); ii != objs.end(); ii++)
		{
			if(AcDbLine::desc() == (*ii)->isA())
			{
				AcGePoint3d start_point = ((AcDbLine*)(*ii))->startPoint();
				AcGePoint3d end_point = ((AcDbLine*)(*ii))->endPoint();

				lenght_move = sqrt(
					(end_point.x - start_point.x)*(end_point.x - start_point.x) + 
					(end_point.y - start_point.y)*(end_point.y - start_point.y) + 
					(end_point.z - start_point.z)*(end_point.z - start_point.z));
				break;
			}
		}

		if(lenght_move < 0.0)
		{
			acutPrintf(_T("No exist a line in the layer!\n"));
			return;
		}

		AcGeMatrix3d transform;
		if(is_move_right)
			transform.setToTranslation(AcGeVector3d(lenght_move, 0.0, 0.0));
		else
			transform.setToTranslation(AcGeVector3d(-lenght_move, 0.0, 0.0));
		
		AcDbEntity* entity = NULL;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();
			entity->transformBy(transform);
			entity->close();
		}
	}

	void Rotate90()
	{
		Acad::ErrorStatus error_status;
		AcDbObjectId layer_id;
		error_status = CheckLayerName(CString(LAYER_NAME), layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the layer!\n"));
			return;
		}

		std::list<AcDbEntity*> objs;
		error_status = GetAllEntities(objs, layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the entities in the layer!\n"));
			return;
		}

		AcGePoint3d point;
		bool is_exist_line = false;
		for(auto ii = objs.begin(); ii != objs.end(); ii++)
		{
			if(AcDbLine::desc() == (*ii)->isA())
			{
				point = ((AcDbLine*)(*ii))->startPoint();
				is_exist_line = true;
				break;
			}
		}

		if(!is_exist_line)
		{
			acutPrintf(_T("No exist a line in the layer!\n"));
			return;
		}

		AcGeMatrix3d transform;
		transform.setToIdentity();
		transform(0, 0) = 0.0;
		transform(1, 1) = 0.0;
		transform(1, 0) = -1.0;
		transform(0, 1) = 1.0;
		
		AcDbEntity* entity = NULL;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();
			entity->transformBy(transform);
			entity->close();
		}
	}

	void DeleteAllBalloons()
	{
		Acad::ErrorStatus error_status;
		AcDbObjectId layer_id;
		error_status = CheckLayerName(CString(LAYER_NAME), layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the layer!\n"));
			return;
		}

		std::list<AcDbEntity*> objs;
		error_status = GetAllEntities(objs, layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the entities in the layer!\n"));
			return;
		}

		AcDbEntity* entity = NULL;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();

			entity->erase();
			entity->close();
		}		
	}

	void AddBalloonTextA()
	{
		AcDbObjectId layer_id = AcDbObjectId::kNull;
		Acad::ErrorStatus error_status = CheckLayerName(CString(LAYER_NAME), layer_id);

		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Not Exist Layer: %s\n"), LAYER_NAME);
			return;
		}

		std::list<AcDbEntity*> objs;
		GetAllEntities(objs, layer_id);
		AcDbEntity* entity;

		AcDbLinetypeTable line_type;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();

			if(AcDbText::desc() == entity->isA())
			{
				ACHAR* text = ((AcDbText*)entity)->textString();
				CString text_added = L"A";
				text_added.Append(CString(&text[0]));
				((AcDbText*)entity)->setTextString(text_added);
			}
			entity->close();
		}
	}

	void CopyBalloonUp()
	{
		static int count = 0;
		count++;
		Acad::ErrorStatus error_status;
		AcDbObjectId layer_id;
		error_status = CheckLayerName(CString(LAYER_NAME), layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the layer!\n"));
			return;
		}

		std::list<AcDbEntity*> objs;
		error_status = GetAllEntities(objs, layer_id);
		if(Acad::eOk != error_status)
		{
			acutPrintf(_T("Can't get the entities in the layer!\n"));
			return;
		}

		double lenght_move = -1.0;
		for(auto ii = objs.begin(); ii != objs.end(); ii++)
		{
			if(AcDbLine::desc() == (*ii)->isA())
			{
				AcGePoint3d start_point = ((AcDbLine*)(*ii))->startPoint();
				AcGePoint3d end_point = ((AcDbLine*)(*ii))->endPoint();

				lenght_move = sqrt(
					(end_point.x - start_point.x)*(end_point.x - start_point.x) + 
					(end_point.y - start_point.y)*(end_point.y - start_point.y) + 
					(end_point.z - start_point.z)*(end_point.z - start_point.z));
				break;
			}
		}

		if(lenght_move < 0.0)
		{
			acutPrintf(_T("No exist a line in the layer!\n"));
			return;
		}

		AcGeMatrix3d transform;
		transform.setToTranslation(AcGeVector3d(0.0, count*3.0*lenght_move, 0.0));
		
		AcDbEntity* entity = NULL;
		AcDbDatabase* database = acdbHostApplicationServices()->workingDatabase();
		AcDbBlockTable* block_table = NULL;
		database->getBlockTable(block_table, AcDb::kForRead);
		AcDbBlockTableRecord* record = NULL;
		block_table->getAt(ACDB_MODEL_SPACE, record, AcDb::kForWrite);
		block_table->close();

		AcDbObjectId obj_id;
		while(!objs.empty())
		{
			entity = objs.front();
			objs.pop_front();

			if(AcDbLine::desc() == ((AcDbLine*)entity)->isA())
			{
				AcDbLine* line = (AcDbLine*)((AcDbLine*)entity)->clone();
				line->transformBy(transform);
				record->appendAcDbEntity(obj_id, line);
				line->close();
			}
			else if(AcDbCircle::desc() == ((AcDbCircle*)entity)->isA())
			{
				AcDbCircle* circle = (AcDbCircle*)((AcDbCircle*)entity)->clone();
				circle->transformBy(transform);
				record->appendAcDbEntity(obj_id, circle);
				circle->close();
			}
			else if(AcDbText::desc() == ((AcDbText*)entity)->isA())
			{
				AcDbText* text = (AcDbText*)((AcDbText*)entity)->clone();
				text->transformBy(transform);
				record->appendAcDbEntity(obj_id, text);
				text->close();
			}


			entity->close();
		}

		record->close();
	}
}


