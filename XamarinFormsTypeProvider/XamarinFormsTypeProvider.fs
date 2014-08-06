namespace Moonmile.FSharp.Lib
open System
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Reflection
open Xamarin.Forms;
open System.Collections.Generic
open System.IO
open System.Xml.Linq

type DataPack() =
    member val Page:Page = null with get, set
    member val Props = new Dictionary<int, obj>() with get, set
type PagePack() = 
    static member val Dic = new Dictionary<int,DataPack>()

    static member SetNames(xml:string) =
        let page = PagePack.Dic.Item(xml.GetHashCode()).Page
        for (name,el) in page.GetNames() do
            PagePack.Dic.Item( xml.GetHashCode() ).Props.Item( name.GetHashCode() ) <- el 

[<TypeProvider>]
type XamarinFormsType(config:TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()
    let namespaceName = "Moonmile.XamarinFormsTypeProvider" 
    let thisAssembly = Assembly.GetExecutingAssembly()


    /// 型生成を残す場合
    /// [<Litelal>]
    /// let xaml = "<ContentPage>...</ContentPage>"
    /// type MainPage = Moonmile.XamarinFormsTypeProvider.XAML< xaml >
    // 型の定義
    let t = ProvidedTypeDefinition(thisAssembly, namespaceName, "XAML", Some(typeof<Xamarin.Forms.Page>), IsErased = false )
    do t.DefineStaticParameters(
        [ProvidedStaticParameter("xaml", typeof<string>)],
        fun typeName parameterValues ->
            let xaml = 
                let str = string parameterValues.[0]
                if str.StartsWith("<") then
                    str
                else
                    let path = config.ResolutionFolder + "\\" + str
                    try
                        System.Diagnostics.Debug.WriteLine("xaml file :'{0}'", path )
                        System.IO.File.ReadAllText(path)
                    with
                    | _ -> 
                        failwithf "Error xaml path %A" path
            
            let outerType = 
                ProvidedTypeDefinition (thisAssembly, namespaceName, 
                    typeName, Some(typeof<Xamarin.Forms.Page>), IsErased = false )
            // テンポラリアセンブリに出力
            let tempAssembly = ProvidedAssembly(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".dll"))
            tempAssembly.AddTypes <| [ outerType ]
            // コンパイル時に試しに読み込んでみる
            let thisPage = Moonmile.FSharp.Lib.PageXaml.LoadXaml<Page>(xaml)
            do PagePack.Dic.Item(xaml.GetHashCode()) <- new DataPack( Page = thisPage )
 
            // コンストラクタの生成
            let ctor = ProvidedConstructor([], 
                            InvokeCode = fun args -> 
                                <@@ 
                                    let page = Moonmile.FSharp.Lib.PageXaml.LoadXaml<Page>(xaml)
                                    PagePack.Dic.Item( xaml.GetHashCode() ) <- new DataPack( Page = page )
                                    PagePack.SetNames(xaml)
                                @@>  )
            do outerType.AddMember( ctor )

            // プロパティを追加
            let prop = 
                ProvidedProperty( "Label", typeof<Xamarin.Forms.Label>, 
                    GetterCode = fun args -> <@@ new Xamarin.Forms.Label() @@> )
            do outerType.AddMember( prop )

            let propXaml = ProvidedProperty(propertyName = "Xaml", 
                                                propertyType = typeof<string>, 
                                                IsStatic=true,
                                                GetterCode= (fun args -> <@@ xaml @@>))
                
            do outerType.AddMember( propXaml )
            let propCurrentPage = ProvidedProperty(propertyName = "CurrentPage", 
                                                propertyType = typeof<Xamarin.Forms.Page>, 
                                                GetterCode= (fun args -> <@@ PagePack.Dic.Item(xaml.GetHashCode()).Page @@> ))
                
            do outerType.AddMember( propCurrentPage )
            
            do
                let page = PagePack.Dic.Item(xaml.GetHashCode()).Page
                let props = PagePack.Dic.Item(xaml.GetHashCode()).Props
                let names = thisPage.GetNames()
                for ( name, el ) in thisPage.GetNames() do
                    props.Add( name.GetHashCode(), null ) // キーのみ確保
                    let prop = ProvidedProperty( name, el.GetType())
                    prop.GetterCode <- fun args -> 
                        <@@ 
                            let props = PagePack.Dic.Item(xaml.GetHashCode()).Props
                            props.Item(name.GetHashCode()) 
                        @@> 
                    outerType.AddMember( prop )

            outerType
    )
    // 名前空間に型を追加
    do this.AddNamespace( namespaceName, [t] )

    override this.ResolveAssembly(args) = 
        let name = System.Reflection.AssemblyName(args.Name)
        let existingAssembly = 
            System.AppDomain.CurrentDomain.GetAssemblies()
            |> Seq.tryFind(fun a -> System.Reflection.AssemblyName.ReferenceMatchesDefinition(name, a.GetName()))
        match existingAssembly with
        | Some a -> a
        | None -> 
            // Fallback to default behavior
            base.ResolveAssembly(args)

