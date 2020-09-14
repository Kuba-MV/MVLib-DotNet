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
#include "IMV_SubtitleParser.h"

/*
Defines visual subtitle item.
Caller can use this struct to render subtitle text
WARNING! Do not modify data of this structure!
*/
struct MV_SubtitleServiceVisualItem
{
	//Pointer to uint8_t bytes. Represents ARGB32 bitmap
	void* data;

	//Width of bitmap
	int w;

	//Height of bitmap
	int h;

	//Pitch of bitmap
	int pitch;

	//Use this method to get empty visual item
	static MV_SubtitleServiceVisualItem GetEmpty()
	{
		MV_SubtitleServiceVisualItem item;

		item.data = NULL;
		item.w = 0;
		item.h = 0;
		item.pitch = 0;

		return item;
	}
};

/*
Defines SubtitleServiceItem
It containts ordinary MV_SubtitleItem with additional data.
Additional data will help you to manage when and how present subtitle to viewer.
WARNING! Do not modify data of this structure!
*/
struct MV_SubtitleServiceItem
{
	//Subtitle item with subtitle data
	MV_SubtitleItem item;

	//Defines if item is empty. If item is empty usually its signal you should stop presenting previous text.
	bool isEmpty;

	//Defines if this item is new one and different than previously obtained. If isNew is true you should remove preview presented text and present this one.
	bool isNew;

	//Use this method to get empty item.
	static MV_SubtitleServiceItem GetEmpty()
	{
		MV_SubtitleServiceItem item;

		item.item = MV_SubtitleItem::GetEmptySubtitleItem();
		item.isNew = false;
		item.isEmpty = true;

		return item;
	}
};

//Defines available subtitle formats
enum MV_SubtitleFormat
{
	//MV_SubtitleService will try to resolve subtitle format automatically
	MV_SubtitleFormatAuto = 0,
	//Force parsing MicroDVD format
	MV_SubtitleFormatMicroDVD = 1,
	//Force parsing SubRip (Srt) format
	MV_SubtitleFormatSubRip = 2,
	//Force parsing MPL2 format
	MV_SubtitleFormatMPL2 = 3,
	//Format was not recognized
	MV_SubtitleFormatUnknown = 100
};

//Defines item color when rendering text or background or outline
struct MV_Color
{
	//red component
	uint8_t r;
	//green component
	uint8_t g;
	//blue component
	uint8_t b;
	//transparency(alpha) component
	uint8_t a;

};

/*
Defines highlevel subtitle service.
This class manages all operations related with resolving subtitle text at given time.
Also you can use this class to render resolved text to ARGB32 bitmap
*/
class IMV_SubtitleService
{
public:

	//Open subtitle file at given format.
	//Currently only UTF-8 files are supported.
	virtual bool OpenSubtitle(const char* fileName, MV_SubtitleFormat format) = 0;

	//Returns item matching given position. Position is in seconds.
	virtual MV_SubtitleServiceItem GetItem(double position) = 0;

	/*
	Render to ARGB32 bitmap provided subtitle item
	lines - strings vector with lines to be rendered
	fontColor - pointer to color structure describing text color
	backColor - pointer to color structure describing background color. Pass NULL if you want transparent background.
	outlineColor - pointer to color structure describing outline font color. Pass NULL if you want text without outline
	You have to call OpenFont method before calling this one.
	*/
	virtual std::vector<MV_SubtitleServiceVisualItem>* RenderVisualItems(std::vector<MV_SubtitleLine>* lines, MV_Color* fontColor, MV_Color * backColor, MV_Color * outlineColor) = 0;

	/*
	Probes subtitle file and returns recognized subtitle format.
	*/
	virtual MV_SubtitleFormat ProbeSubtitles(const char* fileName) = 0;

	/*
	Opens font needed for rendering.
	It must be TTF compatible font.
	fontPath - path to font file
	size - font size in pts
	outlineSize - defines outline size in pts. Pass 0 if you dont want to use outlined font.
	*/
	virtual bool OpenFont(const char * fontPath, int size, int outlineSize) = 0;

	/*
	Closes font.
	*/
	virtual void CloseFont() = 0;

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