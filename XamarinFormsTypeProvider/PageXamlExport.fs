namespace Moonmile.FSharp.Lib
open System.Runtime.CompilerServices
open Xamarin.Forms

[<Extension>]
type PageXaml() =
    static member LoadXaml(xaml:string) = 
        System.Diagnostics.Debugger.Break();
        Moonmile.FSharp.Lib.XForms.ParseXaml.LoadXaml(xaml)

    static member LoadXaml<'T when 'T :> Page >(xaml:string) =
        Moonmile.FSharp.Lib.XForms.ParseXaml.LoadXaml<'T>(xaml)

    static member FindByName(page:Page, name:string) =
        Moonmile.FSharp.Lib.XForms.FindByName(name, page)
    
    /// <summary>
    /// Alias FindByName from Xamarin.Forms
    /// </summary>
    /// <param name="name"></param>
    [<Extension>]
    static member FindByName<'T when 'T :> Element >(this, name:string) =
        Moonmile.FSharp.Lib.XForms.FindByName(name, this) :?> 'T

    [<Extension>]
    static member GetNames( this ) =
        Moonmile.FSharp.Lib.XForms.GetNames(this)

