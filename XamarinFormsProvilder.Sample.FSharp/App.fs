namespace XamarinFormsProvilder.Sample.FSharp
open System
open System.Reflection

open Xamarin.Forms
type App() =
    static member GetMainPage() =
       let main =  new XMain.MainPage()
       // main.text1.Text <- "New F# Xamarin.Forms.TypeProvider"
       let page = main.CurrentPage
       // let page = XMain.MainPageLoad()
       new NavigationPage(  page )
