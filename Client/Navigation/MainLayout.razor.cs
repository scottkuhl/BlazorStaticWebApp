using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Client.Navigation;

public partial class MainLayout
{
	private readonly MudTheme CustomTheme = new();
	private bool _isDarkMode;

	private string DarkModeIcon = Icons.Material.Filled.LightMode;
	private string DrawerBackground = "background-color: transparent";
	private ErrorBoundary? Error;
	private bool IsDrawerOpen = true;
	private MudThemeProvider MudThemeProvider = default!;

	[Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;

	private bool IsDarkMode
	{
		get => _isDarkMode;
		set
		{
			_isDarkMode = value;
			if (_isDarkMode)
			{
				DrawerBackground = $"background-color: {CustomTheme.PaletteDark.Background}";
				DarkModeIcon = Icons.Material.Filled.DarkMode;
			}
			else
			{
				DrawerBackground = $"background-color: {CustomTheme.Palette.Background}";
				DarkModeIcon = Icons.Material.Filled.LightMode;
			}
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			IsDarkMode = await MudThemeProvider.GetSystemPreference();
			StateHasChanged();
		}
	}

	protected override void OnInitialized()
	{
		CustomTheme.Palette.AppbarBackground = CustomTheme.Palette.Background;
		CustomTheme.Palette.AppbarText = CustomTheme.Palette.TextPrimary;

		CustomTheme.PaletteDark.AppbarBackground = CustomTheme.PaletteDark.Background;
	}

	protected override void OnParametersSet()
	{
		Error?.Recover();
	}

	private void DrawerToggle()
	{
		IsDrawerOpen = !IsDrawerOpen;
	}

	private void ModeToggle()
	{
		IsDarkMode = !IsDarkMode;
	}
}