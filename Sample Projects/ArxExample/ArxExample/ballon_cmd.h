#ifndef _BALLOON_CMD_H_
#define _BALLOON_CMD_H_

#include "rxregsvc.h"
#include "acutads.h"
#include "arxHeaders.h"
#include "accmd.h"

class BalloonCmd
{
public:
	BalloonCmd(){}
	~BalloonCmd(){}

	static void InitCmd();
	static void DeleteGroupCmd();

private:
	static AcRxFunctionPtr GetFunctionAddr(const ACHAR* command_name);

private:
	static const ACHAR* cmd_[];
	static const ACHAR* group_name_;
};

#endif