// F# の詳細については、http://fsharp.net を参照してください
// 詳細については、'F# チュートリアル' プロジェクトを参照してください。

[<EntryPoint>]
let main argv = 
    let o = Moonmile.FSharp.Lib.XamarinFormsTypeProvider()
    let ans = o.func( 10 ) 
    printfn "%A" argv
    0 // 整数の終了コードを返します
