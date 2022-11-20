using System.Diagnostics;

namespace Playground;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

    private void SKCanvasView_Touch(object sender, SkiaSharp.Views.Maui.SKTouchEventArgs e)
    {
        Debug.WriteLine(e.ActionType);
    }
}

