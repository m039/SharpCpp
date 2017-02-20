using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CSharpFooBarLibrary;

namespace CSharpConsoleProject
{
	class MainClass
	{
		public static void TestFoo()
		{
			var foo = new Foo();

			Console.Write(
				"== Foo ==\n" +
				"Foo.number: " + foo.GetNumber() + "\n" +
				"Foo.SetNumber(45)\n"
			);

			foo.number = 45;

			Console.Write(
				"Foo.number: " + foo.GetNumber() + "\n"
			);
		}

		public static void Main(string[] args)
		{
			TestFoo();
		}
	}
}
