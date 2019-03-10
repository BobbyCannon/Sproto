#ifndef OSC_SLIP_H
#define OSC_SLIP_H

#include "OscCommon.h"
#include "OscError.h"
#include "OscPacket.h"

/*
 * @brief OSC SLIP decoder buffer size.  If a packet size exceeds the buffer
 * size then all bytes in the decoder buffer will be discarded.
 */
#define OSC_SLIP_DECODER_BUFFER_SIZE (MAX_TRANSPORT_SIZE)

/*
 * @brief OSC SLIP decoder structure.  Structure members are used internally and
 * should not be used by the user application.
 */
typedef struct
{
	char buffer[OSC_SLIP_DECODER_BUFFER_SIZE];
	unsigned int bufferIndex;
	void ( *processPacket)(OscPacket* const oscPacket);
} OscSlipDecoder;

OscError OscSlipEncodePacket(const OscPacket* const oscPacket, size_t* const slipPacketSize, char* const destination,	const size_t destinationSize);
void OscSlipDecoderInitialize(OscSlipDecoder* const oscSlipDecoder);
OscError OscSlipDecoderProcessByte(OscSlipDecoder* const oscSlipDecoder, const char byte);
void OscSlipDecoderClearBuffer(OscSlipDecoder* const oscSlipDecoder);

#endif