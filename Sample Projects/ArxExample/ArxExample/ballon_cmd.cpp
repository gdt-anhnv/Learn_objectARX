#include "stdafx.h"
#include <iostream>
#include <chrono>

#define GROUP_NAME				_T("BalloonCmd")
#define CAL_LENGTH				_T("CalLength")

const ACHAR* BalloonCmd::group_name_ = GROUP_NAME;
const ACHAR* BalloonCmd::cmd_[] = {
	CAL_LENGTH
};

void BalloonCmd::InitCmd()
{
	int num_cmd = sizeof(cmd_)/sizeof(const ACHAR*);
	
	for(int i = 0; i < num_cmd; i++)
	{
		acedRegCmds->addCommand(
			group_name_,
			cmd_[i], 
			cmd_[i],
			ACRX_CMD_MODAL,
			GetFunctionAddr(cmd_[i]));
	}
}

void BalloonCmd::DeleteGroupCmd()
{
	acedRegCmds->removeGroup(group_name_);
}

AcDbObjectIdArray GetObjIdsInSelected()
{
	AcDbObjectIdArray ids = AcDbObjectIdArray();
	ads_name ads;
	acedSSGet(NULL, NULL, NULL, NULL, ads);
	AcDbHandle hndl = AcDbHandle(ads[0], ads[1]);
	if (!hndl.isNull())
		Acad::ErrorStatus err = acedGetCurrentSelectionSet(ids);

	acedSSFree(ads);

	return ids;
}


static void Test()
{
	AcDbObjectIdArray ids = GetObjIdsInSelected();

	auto start = std::chrono::high_resolution_clock::now();

	double total_length = 0.0;
	for (int i = 0; i < ids.length(); i++)
	{
		AcDbObject* obj = nullptr;
		if (Acad::eOk != acdbOpenObject(obj, ids[i], AcDb::kForRead))
			continue;

		AcDbCurve* curve = AcDbCurve::cast(obj);
		if (nullptr == curve)
			continue;

		double len = 0.0;
		curve->getEndParam(len);
		total_length += len;
	}

	acutPrintf(std::to_wstring(total_length).c_str());
	acutPrintf(L"\n");

	auto finish = std::chrono::high_resolution_clock::now();
	acutPrintf(std::to_wstring(std::chrono::duration_cast<std::chrono::nanoseconds>(finish - start).count()).c_str());
}


AcRxFunctionPtr BalloonCmd::GetFunctionAddr(const ACHAR* command_name)
{
	if(_tcscmp(CAL_LENGTH, command_name) == 0)
	{
		acutPrintf(_T("Draw Balloon Function!\n"));
		return Test;
	}
	return NULL;
}
