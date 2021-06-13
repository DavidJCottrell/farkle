module Helpers

open Xunit
open FsCheck
open FsCheck.Xunit
open System

open ScoringLibrary
open GameLibrary

let generateRollWithOneRemainderForDice (diceForRemainder:Dice) =
    let rand = Random()
    let mutable valid = false
    let mutable randomDice = []
    
    let diceToExclude = if diceForRemainder = Dice 1 then Dice 5 else Dice 1
    
    while not valid do
        randomDice <- [
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
        ]
        // Make sure diceForRemainder and diceToExclude doesn't appear in random dice list
        if countOccurrences diceForRemainder randomDice = 0 && countOccurrences diceToExclude randomDice = 0 then
            valid <- true
    Gen.shuffle [diceForRemainder; randomDice.[0]; randomDice.[1]; randomDice.[2]; randomDice.[3]; randomDice.[4]] |> Gen.sample 0 6
    
let generateRollWithOneSetForDice (diceForSet:Dice) =
    let rand = Random()
    let mutable valid = false
    let mutable randomDice = []
    
    // Generate 3 random dice (bar the case of 3 of the same dice as the input resulting in another set)
    while not valid do
        randomDice <- [
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
        ]
        if countOccurrences diceForSet randomDice <= 3 then valid <- true
    
    Gen.shuffle [
        diceForSet
        diceForSet
        diceForSet
        randomDice.[0]
        randomDice.[1]
        randomDice.[2]
    ] |> Gen.sample 0 6



