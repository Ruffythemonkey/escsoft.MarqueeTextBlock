using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace escsoft.Controls;

public sealed partial class MarqueeTextBlock : UserControl
{
	public static readonly DependencyProperty TextProperty =
		DependencyProperty.Register(
			nameof(Text),
			typeof(string),
			typeof(MarqueeTextBlock),
			new PropertyMetadata(string.Empty, OnTextChanged));

	public static readonly DependencyProperty GapProperty =
		DependencyProperty.Register(
			nameof(Gap),
			typeof(double),
			typeof(MarqueeTextBlock),
			new PropertyMetadata(40d, OnLayoutChanged));

	public static readonly DependencyProperty SpeedProperty =
		DependencyProperty.Register(
			nameof(Speed),
			typeof(double),
			typeof(MarqueeTextBlock),
			new PropertyMetadata(60d, OnLayoutChanged)); // Pixel/Sekunde

	private Storyboard? _storyboard;

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public double Gap
	{
		get => (double)GetValue(GapProperty);
		set => SetValue(GapProperty, value);
	}

	public double Speed
	{
		get => (double)GetValue(SpeedProperty);
		set => SetValue(SpeedProperty, value);
	}

	public MarqueeTextBlock()
	{
		InitializeComponent();

		Loaded += (_, _) => UpdateMarquee();
		SizeChanged += (_, e) =>
		{
			ClipRect.Rect = new Windows.Foundation.Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
			UpdateMarquee();
		};
	}

	private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var c = (MarqueeTextBlock)d;

		c.Text1.Text = c.Text;
		c.Text2.Text = c.Text;

		c.UpdateMarquee();
	}

	private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((MarqueeTextBlock)d).UpdateMarquee();
	}

	private void UpdateMarquee()
	{
		if (!IsLoaded)
			return;

		UpdateLayout();

		StopAnimation();

		Text1.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));

		double width = Text1.DesiredSize.Width;

		if (width <= ActualWidth)
		{
			Text2.Visibility = Visibility.Collapsed;
			Translate.X = 0;
			return;
		}

		Text2.Visibility = Visibility.Visible;

		Text1.Margin = new Thickness(0);
		Text2.Margin = new Thickness(Gap, 0, 0, 0);

		double distance = width + Gap;

		var animation = new DoubleAnimation
		{
			From = 0,
			To = -distance,
			Duration = TimeSpan.FromSeconds(distance / Speed),
			RepeatBehavior = RepeatBehavior.Forever
		};

		Storyboard.SetTarget(animation, Translate);
		Storyboard.SetTargetProperty(animation, nameof(TranslateTransform.X));

		_storyboard = new Storyboard();
		_storyboard.Children.Add(animation);
		_storyboard.Begin();
	}

	private void StopAnimation()
	{
		_storyboard?.Stop();
		_storyboard = null;
	}
}