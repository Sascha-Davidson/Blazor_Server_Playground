namespace Playground.FrontEnd.Class
{
	public enum Color
	{
		Default,
		SuperUser,
		Primary,
		Danger
	}

	public static class ColorExtensions
	{
		public static string ToCssClass(this Color color)
		{
			return color switch
			{
				Color.Default => "color-default",
				Color.SuperUser => "color-superuser",
				Color.Primary => "color-primary",
				Color.Danger => "color-danger",
				_ => "color-default"
			};
		}
	}

	public enum Variant
	{
		Solid,
		Faded,
		Bordered,
		Light,
		Flat,
		Ghost,
		Shadow,
		Empty
	}

	public static class VariantExtensions
	{
		public static string ToCssClass(this Variant variant)
		{
			return variant switch
			{
				Variant.Solid => "variant-solid",
				Variant.Faded => "variant-faded",
				Variant.Bordered => "variant-bordered",
				Variant.Light => "variant-light",
				Variant.Flat => "variant-flat",
				Variant.Ghost => "variant-ghost",
				Variant.Shadow => "variant-shadow",
				_ => "variant-solid"
            };
		}
	}
	public enum Size
	{
		Small,
		Medium,
		Large,
	}

	public static class SizeExtensions
	{
		public static string ToCssClass(this Size size)
		{
			return size switch
			{
				Size.Small => "size-small",
				Size.Medium => "size-medium",
				Size.Large => "size-large",
				_ => "size-medium"
            };
		}
	}
}
