# FSharp_Practice
Test projects and playing with FSharp
This code was devised as a rip-off of Donkey Kong, with a historical theme.
The theory was that we controlled a dinosaur at the base of a series of platforms, with the aim of 'eating' the caveman at the top of the screen that was rolling 'wheels' down the platform.

This code was all written within an 8-hour session with myself and Lee attending the Cambridge F# Hackathon.  As a result, the functionality is crude and unfinished, and the code is in dire need of refactoring.

Apart from the problems with the collision detection and the movement, the player object and the 'coins' being moved down the screen ought to share a base class, so that the collision and movement code can be common.

Also, the coin/wheel generation should be done using a lazy sequence, pulling new obstacles out at random intervals.

There are far too many mutable variables for an F# project, although I was starting to wrap the player operations in functional style code - returning new versions of the player object rather than mutating its values each time.
