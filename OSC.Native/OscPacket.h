#ifndef OSC_PACKET_H
#define OSC_PACKET_H

#include "OscBundle.h"
#include "OscCommon.h"
#include "OscError.h"
#include "OscMessage.h"
#include <stddef.h>

/*
 * @brief Maximum OSC packet size.  The OSC packet size is limited by the
 * maximum packet size permitted by the transport layer.
 */
#define MAX_OSC_PACKET_SIZE (MAX_TRANSPORT_SIZE)

/*
 * @brief OSC packet structure.  Structure members are used internally and
 * should not be used by the user application.
 */
typedef struct
{
	char contents[MAX_OSC_PACKET_SIZE];
	size_t size;
	void ( *processMessage)(const OscTimeTag* const oscTimeTag, OscMessage* const oscMessage);
} OscPacket;

void OscPacketInitialize(OscPacket* const oscPacket);
OscError OscPacketInitializeFromContents(OscPacket* const oscPacket, const void* const oscContents);
OscError OscPacketInitializeFromCharArray(OscPacket* const oscPacket, const char* const source, const size_t numberOfBytes);
OscError OscPacketProcessMessages(OscPacket* const oscPacket);

#endif
