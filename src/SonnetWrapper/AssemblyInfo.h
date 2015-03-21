// Copyright (C) 2011, Jan-Willem Goossens 
// All Rights Reserved.
// This code is licensed under the terms of the Eclipse Public License (EPL).

#pragma once

#define VER_FILEVERSION			1,1,2,293
#define VER_FILEVERSION_STR		"1.1.2.293"

// About Production & Assembly version:
// Did the interface change? 
// -> Yes: Are the changes backward compatible?
//         -> No: Then change the version number
//         -> Yes: Keep same version. 
// -> No: Keep same version.
#define VER_PRODUCTVERSION		1,1,2,0
#define VER_PRODUCTVERSION_STR	"1.1.2.0"
#define VER_ASSEMBLYVERSION_STR	"1.1.2.0" // Can use *

#ifdef _DEBUG
#ifdef WIN64
#define VER_FILEDESCRIPTION "SonnetWrapper 64-bit (Debug), based on Cbc 2.9.3"
#else
#define VER_FILEDESCRIPTION "SonnetWrapper 32-bit (Debug), based on Cbc 2.9.3"
#endif
#else
#ifdef WIN64
#define VER_FILEDESCRIPTION "SonnetWrapper 64-bit, based on Cbc 2.9.3"
#else
#define VER_FILEDESCRIPTION "SonnetWrapper 32-bit, based on Cbc 2.9.3"
#endif
#endif

#define VER_COPYRIGHT "Copyright (C) 2011-2015"
#define VER_TRADEMARK "This code is licensed under the terms of the Eclipse Public License (EPL)"
#define VER_FILENAME "SonnetWrapper.dll"
#define VER_FILECOMMENTS "SonnetWrapper is a managed DLL with wrapper classes around existing C++ COIN classes. This version of SonnetWrapper is based on Cbc 2.9.3. See http://sourceforge.net/projects/sonnet-project and http://www.coin-or.org."