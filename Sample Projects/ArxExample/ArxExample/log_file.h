#ifndef _LOG_FILE_H_
#define _LOG_FILE_H_

class Log
{
public:
	Log(){}
	~Log(){}

	static void WriteLog(const char* content);
private:
	static const char* log_file_;
};

#endif