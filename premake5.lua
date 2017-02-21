-- premake5.lua

-- Workapace: CSharpCpp

workspace "CSharpCpp"
  configurations { "Debug", "Release" }
  location "build"
  framework "4.5"

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
  links {
    "Library",
    "System",
    "glib-sharp",
    "gtk-sharp",
    "atk-sharp"
  }

-- Library

project "Library"
  kind "SharedLib"
  language "C#"

  files { "Library/**.cs" }
