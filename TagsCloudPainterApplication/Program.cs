using Autofac;
using ResultLibrary;
using TagsCloudPainter.CloudLayouter;
using TagsCloudPainter.Drawer;
using TagsCloudPainter.FileReader;
using TagsCloudPainter.FormPointer;
using TagsCloudPainter.Parser;
using TagsCloudPainter.Settings;
using TagsCloudPainter.Settings.Cloud;
using TagsCloudPainter.Settings.FormPointer;
using TagsCloudPainter.Settings.Tag;
using TagsCloudPainter.Sizer;
using TagsCloudPainter.Tags;
using TagsCloudPainterApplication.Actions;
using TagsCloudPainterApplication.Infrastructure;
using TagsCloudPainterApplication.Infrastructure.Settings.FilesSource;
using TagsCloudPainterApplication.Infrastructure.Settings.Image;
using TagsCloudPainterApplication.Infrastructure.Settings.TagsCloud;
using TagsCloudPainterApplication.Properties;

namespace TagsCloudPainterApplication;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var container = RegisterModules().OnFail(error => MessageBox.Show("Registrating modules error: " + error));
        if (!container.IsSuccess)
            return;

        var form = GetMainForm(container.GetValueOrThrow())
            .OnFail(error => MessageBox.Show("Form retrieval error: " + error));
        if (!form.IsSuccess)
            return;

        Application.Run(form.GetValueOrThrow());
    }

    private static Result<MainForm> GetMainForm(IContainer container)
    {
        var form = Result.Of(container.Resolve<MainForm>);

        return form;
    }

    private static Result<IContainer> RegisterModules()
    {
        var builder = new ContainerBuilder();
        var tagsCloudPainterLibRegistration = Result.Of(() => builder.RegisterModule(new TagsCloudPainterLibModule()));
        var applicationRegistration = tagsCloudPainterLibRegistration
            .Then(() => builder.RegisterModule(new ApplicationModule()));
        var container = applicationRegistration.Then(() => builder.Build());

        return container;
    }
}

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Palette>().AsSelf().SingleInstance();
        builder.RegisterType<ImageSettings>().As<IImageSettings>().SingleInstance();
        builder.RegisterType<FilesSourceSettings>().As<IFilesSourceSettings>().SingleInstance();
        builder.RegisterType<TagsCloudSettings>().As<ITagsCloudSettings>().SingleInstance();
        builder.RegisterType<PictureBoxImageHolder>().As<IImageHolder, PictureBoxImageHolder>().SingleInstance();
        builder.RegisterType<AppSettings>().As<IAppSettings>().SingleInstance();
        builder.RegisterType<SaveImageAction>().As<IUiAction>();
        builder.RegisterType<PaletteSettingsAction>().As<IUiAction>();
        builder.RegisterType<FileSourceSettingsAction>().As<IUiAction>();
        builder.RegisterType<ImageSettingsAction>().As<IUiAction>();
        builder.RegisterType<DrawTagCloudAction>().As<IUiAction>();
        builder.RegisterType<MainForm>().AsSelf();
    }
}

public class TagsCloudPainterLibModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TextSettings>().As<ITextSettings>().SingleInstance();
        builder.RegisterType<TagSettings>().As<ITagSettings>().SingleInstance();
        builder.RegisterType<SpiralPointerSettings>().As<ISpiralPointerSettings>().SingleInstance();
        builder.RegisterType<CloudSettings>().As<ICloudSettings>().SingleInstance();

        builder.RegisterType<CloudDrawer>().As<ICloudDrawer>().SingleInstance();
        builder.RegisterType<ArchimedeanSpiralPointer>().As<IFormPointer>();
        builder.RegisterType<TagsCloudLayouter>().As<ICloudLayouter>();
        builder.RegisterType<TagsBuilder>().As<ITagsBuilder>().SingleInstance();
        builder.RegisterType<BoringTextParser>().As<ITextParser>().SingleInstance();
        builder.RegisterType<TextFileReader>().As<IFormatFileReader<string>>().SingleInstance();
        builder.RegisterType<TxtFileReader>().As<IFileReader>().SingleInstance();
        builder.RegisterType<DocFileReader>().As<IFileReader>().SingleInstance();
        builder.RegisterType<StringSizer>().As<IStringSizer>().SingleInstance();
        builder.RegisterType<TagSizer>().As<ITagSizer>().SingleInstance();
    }
}