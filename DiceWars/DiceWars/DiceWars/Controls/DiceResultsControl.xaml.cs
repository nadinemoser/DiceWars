using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiceWars.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DiceResultsControl : ContentView
	{
        public string NumberOfDiceText
        {
            get => (string)GetValue(numberOfDiceTextProperty);
            set => SetValue(numberOfDiceTextProperty, value);
        }

        public string UserText
        {
            get => (string)GetValue(userTextProperty);
            set => SetValue(userTextProperty, value);
        }
	    public string Image
	    {
	        get => GetValue(ImageProperty).ToString();
	        set => SetValue(ImageProperty, value);
	    }

        public DiceResultsControl()
	    {
	        InitializeComponent();
	        SizeChanged += OnSizeChanged;
	    }

	    private void OnSizeChanged(object sender, EventArgs e)
	    {
	        bool isPortrait = Height > Width;
	        if (isPortrait)
	        {
	            stacklayout.Orientation = StackOrientation.Vertical;
	            diceNumber.FontSize = 28;
	        }
	        else
	        {
	            stacklayout.Orientation = StackOrientation.Horizontal;
	            diceNumber.FontSize = 20;
	        }
        }

	    private static BindableProperty numberOfDiceTextProperty = BindableProperty.Create(
                                                         propertyName: "NumberOfDiceText",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DiceResultsControl),
                                                         defaultValue: string.Empty,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: numberOfDiceTextPropertyChanged);

        private static void numberOfDiceTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (DiceResultsControl)bindable;
            control.diceNumber.Text = newValue.ToString();
        }

	    private static BindableProperty userTextProperty = BindableProperty.Create(
	        propertyName: "UserText",
	        returnType: typeof(string),
	        declaringType: typeof(DiceResultsControl),
	        defaultValue: "",
	        defaultBindingMode: BindingMode.TwoWay,
	        propertyChanged: userTextPropertyChanged);

	    private static void userTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	    {
	        var control = (DiceResultsControl)bindable;
	        control.user.Text = newValue.ToString();
	    }

	    private static BindableProperty ImageProperty = BindableProperty.Create(
	        propertyName: "Image",
	        returnType: typeof(string),
	        declaringType: typeof(DiceResultsControl),
	        defaultValue: "",
	        defaultBindingMode: BindingMode.TwoWay,
	        propertyChanged: ImagePropertyChanged);

	    private static void ImagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
	    {
	        var control = (DiceResultsControl)bindable;
	        control.image.Source = ImageSource.FromFile(newValue.ToString());
        }
    }
}