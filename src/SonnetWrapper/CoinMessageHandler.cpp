// Copyright (C) 2011, Jan-Willem Goossens 
// All Rights Reserved.
// This code is licensed under the terms of the Eclipse Public License (EPL).

#include "CoinMessageHandler.h"
#include <OsiSolverInterface.hpp>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace COIN
{
	CoinMessageHandlerProxy::CoinMessageHandlerProxy(gcroot<COIN::CoinMessageHandler^> wrapper)
	{
		this->wrapper = wrapper;
	}

	int CoinMessageHandlerProxy::print()
	{
		return wrapper->print();
	}
}