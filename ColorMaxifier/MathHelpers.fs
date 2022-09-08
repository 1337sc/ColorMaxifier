module MathHelpers

type MathHelpers() = 
    static member cutOffNegative number =
        match number with
            | x when x < 0 -> 0
            | x when x >= 0 -> number
            | _ -> number