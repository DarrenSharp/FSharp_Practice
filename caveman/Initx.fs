module Initx

open Library

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
// Game area controls
let gameWidth  = 640.0
let gameHeight = 480.0
let backColor = Colors.LightBlue

// window title
let gameTitle = "Caveman, Score: "

//Target frames per second
let fps = 25

// graphics
//let background = ImageList.LoadImage( "cavemanbackground.jpg" ) //@"C:\Users\Darren\Source\Repos\smallsharp_fc47fc87aa98\caveman\background2.png" )
let coinImage = ImageList.LoadImage ( "coin.png" )
let floorImage = ImageList.LoadImage ( "floor.png" )

// Coins init
let coinsList = [
                    [0.0;1.0]; 
                    [50.0;50.0]
                ]

let coinSpeed = 4.0
let coinFrequency = 4 // Seconds?

// Floor Init
let floorList = [
                    [-50.0;390.0]; 
                    [50.0;290.0]; 
                    [-50.0;160.0]; 
                    [50.0;80.0];
                ]
type Floor = {x:float ; y:float ; angle:float ; img:string}
type Coin = { img:string ; 
              mutable x:float ; 
              mutable y:float ; 
              mutable vx:float ;
              mutable vy:float }
let coinHeight = 20.0
// player
let playerColor = Colors.White
let playerHeight = 45.0
let playerWidth = 20.0
let playerStartx = 550.0
let playerStarty = 350.0
let mutable playerL = ""
let mutable playerR = ""
let mutable playerAngle = 0.0
let mutable playerSpeed = 0.0
let safeTime = 100 // time player has to get out of the way on level up
let playerImage = ImageList.LoadImage( "velociraptor.png" )
let playerImageRight = ImageList.LoadImage("velociraptorRight.png")

let startInit() =
        GraphicsWindow.Hide()
        GraphicsWindow.Title <- gameTitle + "0"
        //GraphicsWindow.CanResize <- "False"
        GraphicsWindow.Width <- int gameWidth
        GraphicsWindow.Height <- int gameHeight
        GraphicsWindow.BackgroundColor <- backColor
        GraphicsWindow.BrushColor <- backColor
        //GraphicsWindow.DrawImage(background, 0, 0)
        //Shapes.AddImage(background) |> ignore
        GraphicsWindow.PenColor <- playerColor
        playerL <- Shapes.AddImage( playerImage )
        playerR <- Shapes.AddImage(playerImageRight)
        // player = Shapes.AddTriangle(playerWidth/2, 0, 0, playerHeight, playerWidth, playerHeight)
        Shapes.Move(playerL, playerStartx, playerStarty)
        Shapes.HideShape(playerR)
        // playerAngle <- 0.0
        let coinList = [
            for i in 1..5 ->
                {img = Shapes.AddImage(coinImage);
                 x = 640.0 - 80.0*(float i) ; y = 50.0 ; vx = -2.0 ; vy=2.0}
            ]       
        let floors = floorList |> List.map( fun f ->
            let a = if f.[0] < 0.0 then 2.0 else -2.0
            let floor = {x=f.[0] ; y = f.[1] ; angle = a ; img = Shapes.AddImage(floorImage)}
            Shapes.Move( floor.img, floor.x, floor.y )
            Shapes.Rotate( floor.img, floor.angle )
            floor )
        (coinList,floors)


