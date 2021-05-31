module Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open System
open Helpers

open ScoringLibrary
open GameLibrary

[<Property>]
let ``A DiceList should never have more than (n - (n % 3)) / 3 sets`` (roll:DiceList) = 
    
    let scoreResults = scoreRoll roll
    
    let mutable numOfSets = 0
    for score in scoreResults do
        match score with
        | SetCombination _ -> numOfSets <- numOfSets + 1
        | _ -> ()
    
    let n = List.length roll
    
    numOfSets <= ((n - (n % 3)) / 3)

[<Fact>]
let ``test that the maximum number of sets for a given DiceList are never exceeded`` () =
    let property num =
        (rollDice (if num > 0 then num else 0)) |> ``A DiceList should never have more than (n - (n % 3)) / 3 sets``
    Check.Quick property
    

[<Fact>]
let ``test that a SetCombination is returned by scoreRoll when given a DiceList with one set`` () =
    // For each dice type
    for i in [1..6] do
        let total = if i = 1 then 300 else i * 100 // Get the total that dice will take as a set
        let generatedRolls = generateRollWithOneSetForDice (Dice i) // Get a list of dice containing a set for that dice and some random other numbers
        let expected = SetCombination([Dice i; Dice i; Dice i], SetTotal total) // Build the SetCombination the actual value should equate to
        // GeneratedSets is a list of 6 combinations for the given dice
        for roll in generatedRolls do
            let actualScores = scoreRoll (roll |> List.ofArray) // Score that generated DiceList
            // Find the SetCombination from the resulting ScoreResults
            let actualSet = List.find (fun score ->
                    match score with
                    | SetCombination (_) -> true
                    | _ -> false) actualScores
            // printfn $"Checking expected %A{expected} against actual  %A{actualSet}"
            // Check it is equal to the expected
            Assert.Equal(expected, actualSet)

[<Fact>]
let ``test that scoreRoll returns one RemainderCombination for the dice provided`` () =
    let remainderOptions = [Dice 1; Dice 5]
    for i in remainderOptions do
        let total = if i = Dice 1 then 100 else 50
        let generatedRolls = generateRollWithOneRemainderForDice i
        let expected = RemainderCombination([i], RemainderTotal total)
        for roll in generatedRolls do
            let actualScores = scoreRoll (roll |> List.ofArray)
            let actualRemainder =
                    List.find (fun score ->
                        match score with
                        | RemainderCombination ([Dice i], _) -> true
                        | _ -> false) actualScores
            //printfn $"Checking expected %A{expected} against actual  %A{actualRemainder}"
            Assert.Equal(expected, actualRemainder)
