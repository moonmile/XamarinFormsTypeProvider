namespace XamarinFormsProvilder.Sample.FSharp

module XMain =
    

    // let st = ResourceLoader.GetObject("MainPage.xml");
    // let xaml = (new System.IO.StreamReader(st)).ReadToEnd()
    
    [<Literal>]
    let XML ="""<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
					   xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
					   x:Class="XFormsPreviewer.MainPage">
    <StackLayout>
        <Label x:Name="text1" Text="Xamrin.Forms Previewer"/>
        <Button x:Name="btn1" Text="Preview" Clicked="OnClickPreview" />
        <Button Text="Grid Preview" Clicked="OnClickGridPreview" />
        <Button Text="AbsoluteLayout Preview" Clicked="OnClickAbsoluteLayoutPreview" />
        <Button Text="Slider Preview" Clicked="OnClickSliderPreview" />
        <Button Text="SliderTrans Preview" Clicked="OnClickSliderTransPreview" />
        <Button Text="Keyboard Preview" Clicked="OnClickKeyboardPreview" />
        <Button Text="load sample 0" Clicked="OnClickSample0" />
    </StackLayout>
</ContentPage>"""

    // type MainPage = Moonmile.XamarinFormsTypeProvider.XAML<XML>
    type MainPage = Moonmile.XamarinFormsTypeProvider.XAML<"MainPage.xaml">
    
    //let MainPageLoad() =
    //    let page = new Moonmile.XamarinFormsDynamic.XamlLoad( XML )
    //    page :> obj :?> Xamarin.Forms.Page

    // let MainPageLoad() = 
    //      Moonmile.FSharp.Lib.PageXaml.LoadXaml<Xamarin.Forms.Page>(XML) 

    // FSC: エラー FS2024: 静的リンクでは、別のプロファイルを対象にしたアセンブリは使用されない場合があります。
    // F# コンパイラのバグらしい
    type MainPageEx(target:MainPage) =
        let mutable count = 0
        do
            target.btn1.Clicked.Add( fun e ->
                count <- count + 1
                target.btn1.Text <- "Clicked " + count.ToString())
        member this.CurrentPage
            with get() = target.CurrentPage
