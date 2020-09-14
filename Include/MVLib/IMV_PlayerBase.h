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

#include "MV_Common.h"

/*
This is base class providing all required features for audiio/video playback.
It uses MV_Decoder to obtain decoder audio/video frames.
Class provides separate thread for audio playback.
Note! Returned video frames are in pure form it can be memory buffer or shared handle to hw texture.
*/
class IMV_PlayerBase
{
public:

	/*
	Returns current player state.
	See MV_PlayerState for detailed documentation.
	*/
	virtual MV_PlayerState GetPlayerState() = 0;

	/*
	Returns current decoding type.
	See MV_DecodingType for detailed documentation.
	*/
	virtual MV_DecodingType GetDecodingType() = 0;

	/*
	Set decoding type.
	Note! You can set decoding type only at NotInitialized and Closed player states.
	*/
	virtual void SetDecodingType(MV_DecodingType decodingType) = 0;

	/*
	Open AVStream.
		pathOrUrl - it can be file location or url to network stream
		returns true on success, otherwise check log for error details
	*/
	virtual void OpenMedia(const char* pathOrUrl) = 0;

	/*
	Open AVStream.
	pathOrUrl - it can be file location or url to network stream
	decodingType - defines decoding type. See MV_DecodingType for details.
	returns true on success, otherwise check log for error details
	*/
	virtual void OpenMedia(const char* pathOrUrl, int packets_buffer_size, int frames_buffer_size) = 0;

	/*
	Returns current volume in range 0.0 to 1.0
	*/
	virtual float GetVolume() = 0;

	/*
	Sets volume from range 0.0 to 1.0
	*/
	virtual void SetVolume(float value) = 0;

	/*
	Sets if frame pooling by player will be strictly aligned to display refresh rate to avoid frame skipping in composition environment.
	Default value is true.
	*/
	virtual void SetVSyncAlignment(bool toggle) = 0;

	/*
	Returns true if player has new pending frame.
	It is auto reset function, value will be set to false after each method invoke.
	Usually this method is uses when SetVSyncAlignment is set to false and caller needs to know if should redraw screen.
	*/
	virtual bool HasNewVideoFrame() = 0;

	/*
	Returns video frame buffer.
	See MV_FrameBuffer for detailed documentation.
	You have to call this method periodically to unqueue video buffers.
	Not calling this method if source has video stream will cause memory growth.
	*/
	virtual MV_FrameBuffer GetVideoFrame() = 0;

	/*
	Returns current source position in seconds.
	*/
	virtual double GetPosition() = 0;

	/*
	Returns current source duration in seconds.
	*/
	virtual double GetDuration() = 0;

	/*
	Seek to provided position.
	value - is position in seconds.
	*/
	virtual void SeekTo(double value) = 0;

	/*
	Close current source and releases memory.
	*/
	virtual void Close() = 0;

	/*
	Returns video widht;
	*/

	virtual int GetVideoWidth() = 0;

	/*
	Returns video height.
	*/
	virtual int GetVideoHeight() = 0;

	/*
	Returns true if source has audio stream.
	*/
	virtual bool HasAudio() = 0;

	/*
	Returns true if source has video stream.
	*/
	virtual bool HasVideo() = 0;

	/*
	Starts playback
	*/
	virtual void Play() = 0;

	/*
	Pauses playback.
	*/
	virtual void Pause() = 0;

	/*
	Stops playback and rewinds to the begining of source.
	*/
	virtual void Stop() = 0;

	/*
	Flushes all demuxed and decoded buffers.
	*/
	virtual void FlushAllBuffers() = 0;

	/*
	Flushes all demuxed and decoded audio buffers.
	*/
	virtual void FlushAudioBuffers() = 0;

	/*
	Flushes all demuxed and decoded video buffers.
	*/
	virtual void FlushVideoBuffers() = 0;
	
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
