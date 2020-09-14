/*
  Media Vault Library
  Copyright (C) 2020 Jakub Gluszkiewicz (kubabrt@gmail.com)
  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  You may use this code under license agreements which can be found at www.libmediavault.com.
  You may use this code and referenced libraries for free without any limitations and any fees under following conditions:
  - your derative work is for non-commercial purposes
  - your derative work is for commercial purposes but your annual company income is not greater than 50K $ (american dollars)
  Otherwise please contact me directly to buy license for commmercial use.
*/

#pragma once

#include "MV_DLLExport.h"
#include "MV_Common.h"
#include <string>
#include <vector>

/*
Defines subtitle line
WARNING! Do not modify data of this structure!
*/
struct MV_SubtitleLine
{
	//Represents subtitle line
	std::string line;

	//defines if presenting line should be in bold font style
	bool isBold;

	//defines if presenting line should be in italic font style
	bool isItalic;
};

/*
Defines subtitle item.
WARNING! Do not modify data of this structure!
*/
struct MV_SubtitleItem
{
	//defines when text should be presented to viewer; unit is seconds
	double startTime;

	//defines when text should be stopped presenting to viewer; unit is seconds
	double endTime;

	//pointer to vector with subtitle lines which should be presented in given timestamp
	std::vector<MV_SubtitleLine>* lines;

	//use this method to get empty SubtitleItem
	static MV_SubtitleItem GetEmptySubtitleItem()
	{
		MV_SubtitleItem item;
		item.startTime = -1.0f;
		item.endTime = -1.0f;
		item.lines = NULL;

		return item;
	}

};

/*
Defines subtitle parser interface. Regardless of subtitle file format factory always will return this interface.
*/
class IMV_SubtitleParser
{
public:

	//Parse subtitle file and returns pointer to vector with subtitle items
	//Pointer is valid till you will call this method again or you will release object
	//Currently only UTF-8 files are supported.
	virtual std::vector<MV_SubtitleItem>* ParseStream(const char* fileName) = 0;

	/*
	Add refenrence to object
	*/
	virtual void AddRef() = 0;

	/*
	Remove reference to object.
	If reference counted will be 0 object will be deleted.
	*/
	virtual void Release() = 0;

};