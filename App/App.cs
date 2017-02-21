using Gtk;
using System;

class Hello
{

	static void Main()
	{
		Application.Init();

		Window window = new Window("helloworld");
		window.Resize(800, 800);
		window.Show();

		Application.Run();

	}
}