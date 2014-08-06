namespace Moonmile.FSharp.Lib
open System
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Reflection
open Xamarin.Forms;
open System.Collections.Generic
open System.IO

[<TypeProvider>]
type XamarinFormsDynamic() as this = 
    inherit TypeProviderForNamespaces()
    let namespaceName = "Moonmile.XamarinFormsDynamicProvider" // this.GetType().Namespace
    let thisAssembly = Assembly.GetExecutingAssembly()
    
    let XAML = "1234" // 識別用の番号

    /// 型を消去する場合
    let outerType = 
        ProvidedTypeDefinition(thisAssembly, namespaceName, 
            "XamlLoad", Some(typeof<obj>), IsErased = true)
    // コンストラクタの生成
    let ctor = ProvidedConstructor([ProvidedParameter("xaml", typeof<string>)], 
                    InvokeCode = fun args -> 
                        <@@ 
                            let xaml = (%%(args.[0]):string)
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
                                        GetterCode= (fun args -> <@@ XAML @@>))
                
    do outerType.AddMember( propXaml )
    let propCurrentPage = ProvidedProperty(propertyName = "CurrentPage", 
                                        propertyType = typeof<Xamarin.Forms.Page>, 
                                        GetterCode= (fun args -> <@@ PagePack.Dic.Item(XAML.GetHashCode()).Page @@> ))
                
    do outerType.AddMember( propCurrentPage )
    
    (*
    do
        let page = PagePack.Dic.Item(XAML.GetHashCode()).Page
        let props = PagePack.Dic.Item(XAML.GetHashCode()).Props
        let names = page.GetNames()
        for ( name, el ) in page.GetNames() do
            props.Add( name.GetHashCode(), null ) // キーのみ確保
            let prop = ProvidedProperty( name, el.GetType())
            prop.GetterCode <- fun args -> 
                <@@ 
                    let props = PagePack.Dic.Item(XAML.GetHashCode()).Props
                    props.Item(name.GetHashCode()) 
                @@> 
            outerType.AddMember( prop )

    *)
    (*    
    // プロパティを追加
    let prop = 
        ProvidedProperty( "prop", typeof<int>, 
            GetterCode = fun args -> <@@ 999 @@> )
    do t.AddMember( prop )

    // メソッドを追加
    let meth =
            ProvidedMethod( 
                "func", [ProvidedParameter("x", typeof<int>)], typeof<int>, 
                InvokeCode = fun args ->
                    <@@  (%%(args.[1]):int) + 10  @@> )
    do t.AddMember( meth )
    *)

    // 名前空間に型を追加
    do this.AddNamespace( namespaceName, [outerType] )
(*
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
*)
