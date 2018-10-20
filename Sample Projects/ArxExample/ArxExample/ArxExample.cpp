// ArxExample.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


extern "C" AcRx::AppRetCode acrxEntryPoint(AcRx::AppMsgCode msg, void* appId)
{
	switch(msg) 
	{
	case AcRx::kInitAppMsg:
		acrxUnlockApplication(appId);
		acrxRegisterAppMDIAware(appId);
		acutPrintf(_T("\n Minimum ObjectARX application (by Anh.NQV) loaded! \n"));
		BalloonCmd::InitCmd();
		break;
	case AcRx::kUnloadAppMsg:
		acutPrintf(_T("\n Minimum ObjectARX application (by Anh.NQV) unloaded! \n"));
		BalloonCmd::DeleteGroupCmd();
		break;
	}

	return AcRx::kRetOK;
}

