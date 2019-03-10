#pragma once

#include "..\OSC.Native\Osc99.h"

namespace Microsoft
{
	namespace VisualStudio
	{
		namespace CppUnitTestFramework
		{
			template <> static std::wstring ToString<OscError>(const OscError& t)
			{
				RETURN_WIDE_STRING(t);
			}

			template <> static std::wstring ToString<OscTypeTag>(const OscTypeTag& t)
			{
				RETURN_WIDE_STRING(t);
			}

			template <> static std::wstring ToString<uint16_t>(const uint16_t& t)
			{
				RETURN_WIDE_STRING(t);
			}
		}
	}
}
