module Playx

open Library
open Microsoft.FSharp.Core.Operators

let log x = 
    System.Diagnostics.Debug.WriteLine x
    System.Console.WriteLine( x )

let fps = 20

// Key controls
let leftKey  = "Left"
let rightKey = "Right"
let forwardKey = "Up"
let backKey = "Down"
let fireKey = "Space"
let pauseKey = "P"

let mutable play = 0
let mutable playerSafe = 0
let mutable score = 0
let bottom = 420.0 - Initx.playerHeight
let radians degrees = degrees * System.Math.PI / 180.

type Player = { imgL:string ; imgR:string ; x:float ; y:float ; vy:float ; crouch:bool ; bash:bool ; faceLeft:bool }
let mutable player = {imgL=Initx.playerL ;imgR=Initx.playerR ;x=Initx.playerStartx ; y=Initx.playerStarty ; vy=0.0 ; bash=false ; crouch=false ; faceLeft=true}    

let Gravity( player ) =
    {player with y = player.y + player.vy ; vy = player.vy + 5.0}
let Limit( player ) =
    let newX =
        if player.x < 0.0 then 0.0 elif player.x > 600.0 then 600.0 else player.x
    let newY = if player.y < 0.0 then 0.0 elif player.y > bottom then bottom else player.y
    {player with x=newX ; y=newY}
let FloorDetect floors player =
    let TryInFloor (floor:Initx.Floor) = 
        // calculate the location of containing the floor structure
        let px = player.x - floor.x
        let floorY = floor.y + px * tan(radians (floor.angle))
        if floorY > player.y && floorY < player.y + Initx.playerHeight then
            // adjust the player to be on the floor
            true, floorY - Initx.playerHeight
        else false, 0.0
    match floors |> List.map ( TryInFloor ) |> List.filter( fun (t, f) -> t ) with
    | [(true, floorY)] -> {player with y = floorY}
    | _ -> player

let coinFloorDetect (floors) (coin:Initx.Coin) =
    let TryInFloor (floor:Initx.Floor) =
        let px = coin.x - floor.x
        let floorY = floor.y + px * tan(radians( floor.angle))
        if floor.x < 0.0 && coin.x > 560.0 then 
            false,0.0
        elif floor.x > 0.0 && coin.x < 40.0 then
            false, 0.0
        elif floorY > coin.y && floorY < coin.y + Initx.coinHeight then
            true, coin.y - Initx.coinHeight
        else false, 0.0
    match floors |> List.map ( TryInFloor ) |> List.filter( fun (t,f) -> t ) with
    |[(true, floorY)] -> 
        coin.y <- floorY
        0
    | _ -> 0
let moveCoin (floors) (coin:Initx.Coin) =
    let x = coin.x + coin.vx
    if x < 10.0 then 
        coin.vx <- -coin.vx
        coin.x <- 10.0
    elif x > 640.0 then
        coin.vx <- -coin.vx
        coin.x <- 640.0
    else
        coin.x <- x 
    let y = coin.y + coin.vy
    if y > 480.0 then coin.y <- 0.0 else coin.y <- y
    coinFloorDetect floors coin

let rec Init() =
    Initx.startInit()
and Play( coins, floors ) =
    GraphicsWindow.Show()
    GraphicsWindow.KeyDown <- Callback(ChangeDirection)
    // Main loop
    play <- 1
    while (play = 1) do
        Program.Delay(1000/fps)
        Move(coins,floors)
        CollisionCheck(coins, floors)
   // Read key event and act
and ChangeDirection () = 
    if (GraphicsWindow.LastKey = rightKey) then
        "move right" |> log
        let vy = if player.vy < 0.0 then player.vy else player.vy - 4.0
        player <- {player with x = player.x + 2.0 ; vy = vy ; faceLeft=true}
    elif (GraphicsWindow.LastKey = leftKey) then
        "move left" |> log
        let vy = if player.vy < 0.0 then player.vy else player.vy - 4.0
        player <- {player with x = player.x - 2.0 ; vy = vy ; faceLeft=false}
    elif (GraphicsWindow.LastKey = forwardKey) then
        if player.vy = 0.0 then
            "jump" |> log
            player <- {player with vy = -20.0 ; y = player.y - 5.0}
        else
            "flap" |> log
            player <- {player with vy = -5.0}
    elif (GraphicsWindow.LastKey = backKey) then
        "crouch" |> log
        player <- {player with crouch = true}
    elif (GraphicsWindow.LastKey = fireKey) then
        "bash" |> log
        player <- {player with bash = true}

and Move(coins:Initx.Coin list,floors) =
    // move the player
    player <- Gravity( player ) |> Limit |> FloorDetect floors
    if player.faceLeft then
        Shapes.ShowShape(player.imgR)
        Shapes.HideShape(player.imgL)
    else
        Shapes.ShowShape(player.imgL)
        Shapes.HideShape(player.imgR)
    coins |> List.map( fun c ->
        moveCoin floors c |> ignore
        Shapes.Move( c.img, c.x, c.y ) )|> ignore
    // create a line representing the floor
    Shapes.Move( player.imgL, player.x, player.y )
    Shapes.Move( player.imgR, player.x, player.y )
    // move the coins

and CollisionCheck(floors) =
    let mutable px1 = 0.0
    let mutable py1 = 0.0
    let mutable px2 = 0.0
    let mutable py2 = 0.0
    // Calculate player bounding box.
    px1 <- Shapes.GetLeft(player.imgL) - (Initx.playerWidth / 2.0)
    py1 <- Shapes.GetTop(player.imgL) - (Initx.playerHeight / 2.0)
    px2 <- px1 + Initx.playerWidth
    py2 <- py1 + Initx.playerHeight
