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
This is demuxer and decoder class.
It doesn't provide any playback logic.
Decoder expose methods to access decoded audio/video frames on demand.
*/
class IMV_Decoder
{
public:

	/*
	Return current decoder state.
	See MV_DecoderState enumeration description for more details.
	*/
	virtual MV_DecoderState GetDecoderState() = 0;

	/*
	Open AVStream.
	pathOrUrl - it can be file location or url to network stream
	returns true on success, otherwise check log for error details
	*/
	virtual bool OpenAVStream(const char* pathOrUrl) = 0;

	/*
	Open AVStream.
	pathOrUrl - it can be file location or url to network stream
	decodingType - defines decoding type. See MV_DecodingType for details.
	returns true on success, otherwise check log for error details
	*/
	virtual bool OpenAVStream(const char* pathOrUrl, MV_DecodingType decodingType) = 0;

	/*
	Returns video width.
	*/
	virtual int GetVideoWidth() = 0;

	/*
	Returns video height.
	*/
	virtual int GetVideoHeight() = 0;

	/*
	Returns true if all buffers (audio/video) were decoded and unqueued by client.
	*/
	virtual bool BuffersCompleted() = 0;

	/*
	Seeks to provided location in stream.
	Note that during seek operation deocder state is changing to NotInitialized at the begining and when seek operation is completed state is changed to Opened
	*/
	virtual void SeekTo(double position, bool isBackward) = 0;

	/*
	Returns true if source has audio stream.
	*/
	virtual bool HasAudio() = 0;

	/*
	Returns true if source has video stream.
	*/
	virtual bool HasVideo() = 0;

	/*
	Returns number of audio channels.
	*/
	virtual int GetAudioChannels() = 0;

	/*
	Returns audio frequency.
	*/
	virtual int GetAudioFreq() = 0;

	/*
	Returns resample rate. It's used to define packet presentation timestamp.
	*/
	virtual double GetAudioResampleRate() = 0;

	/*
	Returns video frame rate.
	*/
	virtual double GetVideoFrameRate() = 0;

	/*
	Add refenrence to object
	*/
	virtual void AddRef() = 0;

	/*
	Remove reference to object.
	If reference counted will be 0 object will be deleted.
	*/
	virtual void Release() = 0;

	/*
	Unqeue pending audio frame.
	See MV_FrameBuffer for more details.
	Note! You have to flush audio frame to unqueue next audio buffer.
	*/
	virtual MV_FrameBuffer GetAudioFrame() = 0;

	/*
	Releases current audio buffer.
	After calling this method, you can unqueue next buffer.
	*/
	virtual void FlushAudioFrame() = 0;

	/*
	Unqeue pending video frame.
	See MV_FrameBuffer for more details.
	Note! You have to flush video frame to unqueue next video buffer.
	*/
	virtual MV_FrameBuffer GetVideoFrame() = 0;

	/*
	Releases current video buffer.
	After calling this method, you can unqueue next buffer.
	*/
	virtual void FlushVideoFrame() = 0;

	/*
	Returns source duration in seconds.
	*/
	virtual double GetDuration() = 0;

	/*
	Flushes immediatelly all decoded and demuxed audio and video buffers
	*/
	virtual void FlushAllBuffers() = 0;

	/*
	Flushes immediatelly all decoded and demuxed audio buffers
	*/
	virtual void FlushAudioBuffers() = 0;

	/*
	Flushes immediatelly all decoded and demuxed video buffers
	*/
	virtual void FlushVideoBuffers() = 0;

};