module Helpers

open Xunit
open FsCheck
open FsCheck.Xunit
open System

open ScoringLibrary
open GameLibrary

let expectedPossibleSets:ScoreResults = [
    SetCombination ([Dice 1; Dice 1; Dice 1], SetTotal 300);
    SetCombination ([Dice 2; Dice 2; Dice 2], SetTotal 200);
    SetCombination ([Dice 3; Dice 3; Dice 3], SetTotal 300);
    SetCombination ([Dice 4; Dice 4; Dice 4], SetTotal 400);
    SetCombination ([Dice 5; Dice 5; Dice 5], SetTotal 500);
    SetCombination ([Dice 6; Dice 6; Dice 6], SetTotal 600);
]

let expectedPossibleRemainders:ScoreResults = [
    RemainderCombination ([Dice 1], RemainderTotal 100);
    RemainderCombination ([Dice 5], RemainderTotal 50);
    RemainderCombination ([Dice 1; Dice 1], RemainderTotal 200);
    RemainderCombination ([Dice 5; Dice 5], RemainderTotal 100);
]

(*
let generateRollWithOnlyOneRemainderCombination =
    let rand = Random()
    let mutable valid = false
    let mutable randomDice = []
    
    while not valid do
        randomDice <- [
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
        ]
        if countOccurrences diceForRemainder randomDice = 0 then valid <- true
    Gen.shuffle [diceForRemainder; randomDice.[0]; randomDice.[1]; randomDice.[2]; randomDice.[3]; randomDice.[4]] |> Gen.sample 0 6

*)

let generateRollWithOneRemainderForDice (diceForRemainder:Dice) =
    let rand = Random()
    let mutable valid = false
    let mutable randomDice = []
    
    while not valid do
        randomDice <- [
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
        ]
        if countOccurrences diceForRemainder randomDice = 0 then
            valid <- true
    Gen.shuffle [diceForRemainder; randomDice.[0]; randomDice.[1]; randomDice.[2]; randomDice.[3]; randomDice.[4]] |> Gen.sample 0 6

let generateRollWithTwoRemaindersForDice (diceForRemainder:Dice) =
    let rand = Random()
    let mutable valid = false
    let mutable randomDice = []
    
    while not valid do
        randomDice <- [
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
            Dice (rand.Next (1, 7))
        ]
        if countOccurrences diceForRemainder randomDice = 0 then valid <- true
    let diceList:DiceList = [diceForRemainder; randomDice.[0]; randomDice.[1]; randomDice.[2]; randomDice.[3]; diceForRemainder]
    Gen.shuffle diceList
    |> Gen.sample 0 6
    
let generateRollWithOneSetForDice (diceForSet:Dice) =
    let rand = Random()
    let mutable valid = false
    let mutable randomDice = []
    
    // Generate 3 random dice (bar the case of 3 of the same dice as the input resulting in another set)
    while not valid do
        randomDice <- [Dice (rand.Next (1, 7)); Dice (rand.Next (1, 7)); Dice (rand.Next (1, 7))]
        if countOccurrences diceForSet randomDice < 3 then valid <- true
    
    Gen.shuffle [diceForSet; diceForSet; diceForSet; randomDice.[0]; randomDice.[1]; randomDice.[2]] |> Gen.sample 0 6



