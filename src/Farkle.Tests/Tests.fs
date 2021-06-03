module Tests

open Xunit
open FsCheck
open FsCheck.Xunit
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
    for i in [1..6] do
        let expectedTotal = if i = 1 then SetTotal 300 else SetTotal (i * 100)
        let expectedDice = [Dice i; Dice i; Dice i]
        let expectedSet = SetCombination(expectedDice, expectedTotal)
        
        // Get a list of dice containing a set for that dice and some random other numbers
        let generatedRolls = generateRollWithOneSetForDice (Dice i)
        
        // GeneratedSets is a list of 6 combinations for the given dice
        for roll in generatedRolls do
            let actualScores = scoreRoll (roll |> List.ofArray)
            
            // Find the SetCombination from the resulting ScoreResults
            let actualSet = List.find (fun score ->
                    match score with
                    | SetCombination (_) -> true
                    | _ -> false) actualScores
            
            Assert.Equal(expectedSet, actualSet)

[<Fact>]
let ``test that scoreRoll returns one RemainderCombination for the dice provided`` () =
    let remainderPossibilities = [Dice 1; Dice 5]
    for i in remainderPossibilities do
        
        let expectedTotal = if i = Dice 1 then RemainderTotal 100 else RemainderTotal 50
        let expectedRemainder = RemainderCombination([i], expectedTotal)
        
        let generatedRolls = generateRollWithOneRemainderForDice i
        
        for roll in generatedRolls do
            let actualScores = scoreRoll (roll |> List.ofArray)
            let actualRemainder =
                    List.find (fun score ->
                        match score with
                        | RemainderCombination ([Dice i], _) -> true
                        | _ -> false) actualScores
                    
            Assert.Equal(expectedRemainder, actualRemainder)


// -- Current complete tests --
// - making sure the max number of sets are never exceeded
// - making sure that a SetCombination is actually returned when you expect it to be
// - making sure that a RemainderCombination is actually returned when you expect it to be

// -- Tests yet to be complete --
// - 



