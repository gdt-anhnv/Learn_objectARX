#include "stdafx.h"
#include <fstream>
#include <iostream>


const char* Log::log_file_ = "log.txt";

void Log::WriteLog(const char* content)
{
	std::ofstream log;
	log.open(Log::log_file_, std::ios::in | std::ios::trunc);

	if(log.is_open())
	{
		log << content << std::endl;
		log.close();
	}
}