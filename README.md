# There Is No Such Thing As A Free Lunch
Calculates amount owed per person in a group meal order, accounting for tax &amp; tip/fees.

# Input File Instructions
- The input file must be a text file (*.txt) in the same directory as the executable and config file.
- Each line must contain the **name** of an order participant, followed by a single space, followed by the summed **amount** of that participant's order item(s).
- One line must contain *only* an **amount**, which will be used as the tip/fees amount to be split amongst each of the order participants. If none is specified, it will default to 0.

Sample input .txt file:

```
Chris 12.25
Sam 14.40
Forrest 7.65
Maxwell 4.20
9.50
```

# Config File Options
- **taxRate**: Percent tax to be applied.
- **prorated**: Set to "true" to charge participants for tip/fees individually based on the amount they ordered. Set to "false" to divide it up evenly.
