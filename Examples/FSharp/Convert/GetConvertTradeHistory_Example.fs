open System.Net
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Binance.Common
open Binance.Spot
open Binance.Spot.Models

[<EntryPoint>]
let main argv =
    let loggerFactory = LoggerFactory.Create(fun (builder:ILoggingBuilder) -> 
        builder.AddConsole() |> ignore
    )
    let logger = loggerFactory.CreateLogger()

    let loggingHandler = new BinanceLoggingHandler(logger)
    let httpClient = new HttpClient(loggingHandler)
    
    let convert = new Convert(httpClient)
    
    let result = convert.GetConvertTradeHistory(1639646747000, 1642325147000) |> Async.AwaitTask |> Async.RunSynchronously
    
    0
