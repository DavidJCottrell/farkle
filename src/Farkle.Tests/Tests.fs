module Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open Helpers

open ScoringLibrary
open GameLibrary

[<Property>]
let ``A DiceList should never have more than (n - (n % 3)) / 3 sets``
    (roll:DiceList) = 
    let scoreResults = scoreRoll roll
    
    let mutable numOfSets = 0
    for score in scoreResults do
        match score with
        | SetCombination _ -> numOfSets <- numOfSets + 1
        | _ -> ()
    
    let n = List.length roll
    
    numOfSets <= ((n - (n % 3)) / 3)


[<Fact>]
let ``test that scoreRoll never produces more than (n - (n % 3)) / 3 sets`` () =
    let property num =
        // Never roll negative dice
        (rollDice (if num > 0 then num else 0))
        |> ``A DiceList should never have more than (n - (n % 3)) / 3 sets``
    Check.Quick property


[<Fact>]
let ``test for correct SetCombination`` () =
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
                    | SetCombination (expectedDice, expectedTotal) -> true
                    | _ -> false) actualScores
                
            Assert.Equal(expectedSet, actualSet)

[<Fact>]
let ``test for correct RemainderCombination`` () =
    let remainderPossibilities = [Dice 1; Dice 5]
    for expectedDice in remainderPossibilities do
        
        let expectedTotal = if expectedDice = Dice 1 then RemainderTotal 100 else RemainderTotal 50
        let expectedRemainder = RemainderCombination([expectedDice], expectedTotal)
        
        let generatedRolls = generateRollWithOneRemainderForDice expectedDice
        
        for roll in generatedRolls do
            let actualScores = scoreRoll (roll |> List.ofArray)
            let actualRemainder =
                    List.find (fun score ->
                        match score with
                        | RemainderCombination ([expectedDice], expectedTotal) -> true
                        | _ -> false) actualScores 
            Assert.Equal(expectedRemainder, actualRemainder)

[<Fact>]
let ``test for correct RemainderCombination and SetCombination`` () =
    let testRolls = [
        [Dice 1; Dice 2; Dice 1; Dice 3; Dice 3; Dice 4]
        [Dice 5; Dice 1; Dice 5; Dice 4; Dice 4; Dice 4]
        [Dice 6; Dice 6; Dice 6; Dice 6; Dice 6; Dice 6]
        [Dice 5; Dice 4; Dice 1; Dice 2; Dice 1; Dice 1]
        [Dice 3; Dice 1; Dice 3; Dice 3; Dice 3; Dice 3]
        [Dice 2; Dice 2; Dice 2; Dice 6; Dice 5; Dice 5]
    ]
    
    let expectedResults = [
        [
            RemainderCombination([Dice 1; Dice 1], RemainderTotal 200)
        ];
        [
            RemainderCombination([Dice 1], RemainderTotal 100)
            SetCombination([Dice 4; Dice 4; Dice 4], SetTotal 400)
            RemainderCombination([Dice 5; Dice 5], RemainderTotal 100)
        ];
        [
            SetCombination([Dice 6; Dice 6; Dice 6], SetTotal 600);
            SetCombination([Dice 6; Dice 6; Dice 6], SetTotal 600)
        ]
        [
            SetCombination([Dice 1; Dice 1; Dice 1], SetTotal 300)
            RemainderCombination([Dice 5], RemainderTotal 50)
        ]
        [
            RemainderCombination([Dice 1], RemainderTotal 100)
            SetCombination([Dice 3; Dice 3; Dice 3], SetTotal 300)
        ]
        [
            SetCombination([Dice 2; Dice 2; Dice 2], SetTotal 200);
            RemainderCombination([Dice 5; Dice 5], RemainderTotal 100)
        ]
    ]
    
    for i in [0..(testRolls.Length - 1)] do
        Assert.True(expectedResults.[i] = (scoreRoll testRolls.[i]))