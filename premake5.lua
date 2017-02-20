-- premake5.lua

-- Workapace: CSharpCpp

workspace "CSharpCpp"
  configurations { "Debug", "Release" }
  location "build"

  filter "configurations:Debug"
  defines { "DEBUG" }
  symbols "On"

  filter "configurations:Release"
  optimize "On"

-- App

project "App"
  kind "WindowedApp"
  language "C#"

  files { "App/**.cs" }
  links { "Library" }

-- Library

project "Library"
  kind "SharedLib"
  language "C#"

  files { "Library/**.cs" }

