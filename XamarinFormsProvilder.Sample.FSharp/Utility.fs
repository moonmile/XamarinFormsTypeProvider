namespace XamarinFormsProvilder.Sample.FSharp

open System
open System.Linq;
open System.Reflection


type ResourceLoader() =
    static member val Names = [||] with get, set
    static member val Asm:Assembly = null with get, set

    static member GetObject(resourceName:string) =
        if  ResourceLoader.Asm = null then
            ResourceLoader.Asm <- typeof<ResourceLoader>.GetTypeInfo().Assembly
            ResourceLoader.Names <- ResourceLoader.Asm.GetManifestResourceNames()
        
        try 

            let path = ResourceLoader.Names.First(fun x -> x.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase))
            ResourceLoader.Asm.GetManifestResourceStream(path)
        with
        | _ -> null



