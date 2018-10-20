#ifndef _UTILS_H_
#define _UTILS_H_

#include "stdafx.h"

namespace Utils
{
	Acad::ErrorStatus CheckLayerName(const CString& layer_name, AcDbObjectId& layer_id);

	AcDbObjectId CreateNewLayer(const CString& layer_name, const int& color_index);

	Acad::ErrorStatus MakeGroup(const AcDbObjectIdArray& object_id_array, AcDbObjectId& group_id);

	Acad::ErrorStatus GetAllEntities(AcDbObjectIdArray& id_arr, const AcDbObjectId& layer_id);

	AcDbLayerTableRecord* GetLayerById_ForWrite(const AcDbObjectId& layer_id);

	void GroupBalloon();

	void DrawBalloons();

	void ChangeColorBallons();

	void ChangeLineType();

	void MoveHozirontal(const bool& is_move_right);

	void Rotate90();

	void DeleteAllBalloons();

	void AddBalloonTextA();

	void CopyBalloonUp();
}

#endif