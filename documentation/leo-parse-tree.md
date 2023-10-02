# Leo Parse Tree

The Elizabeth Scott Paper titled "SPPF-Style Parsing From Earley Recognisers" (see repo README.md for link) covers parse tree creation for normal earley parsers. 

I was unable to find a paper that covers the parse tree creation for leo items after applying the optimization from the Joop Leo paper titled "A general context-free parsing algorithm running in linear time on every LR(k) grammar without using lookahead" (see repo README.md for link).

This document will demostrate the data structures and algorithms used by Pliant to create a dynamic parse tree for leo items.

## The problem

Because Leo Items provide a cached version of an Earley reduction path, all parse nodes in that reduction path, excluding the top and bottom, are missing from the chart. 

## Example 

The following example shows the same parse performed with leo optimization and using normal earley. 

### Grammar

```
E -> F
E -> F E
E -> 
F -> 'a''
```

### Input

```
aaaa
```

### Chart

| 0				| 0 - nodes | 1				 | 1 - nodes	 | 2	         | 2 - nodes	 | 3		     | 3 - nodes	 | 4		     | 4 - nodes     |
| ------------- | --------- | -------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| E -> * F, 0   |			| F -> 'a' *, 0  | (F, 0, 1)     |               |               |               |               |               |               |
| E -> * F E, 0 |			| E -> F *, 0    | (E, 0, 1)     |               |               |               |               |               |               |
| E -> *, 0		| (E, 0, 0)	| E -> F * E, 0  |               |               |               |               |               |               |               |
| F -> * 'a', 0 |			| E -> * F, 0	 |               |               |               |               |               |               |               |
|               |			| E -> * F E, 1  |               |               |               |               |               |               |               |
|               |			| E -> * F, 1    |               |               |               |               |               |               |               |
|               |			| E -> *, 1      | (E, 1, 1)     |               |               |               |               |               |               |

### Parse Forest (Breadth First)

```
(E, 0, 5) -> (E, 0, 1) (E, 1, 5)
(E, 0, 4) -> (E, 0, 1) (E, 1, 4)
(E, 1, 5) -> (E, 1, 2) (E, 2, 5)
(E, 0, 3) -> (E, 0, 1) (E, 1, 3)
(E, 1, 4) -> (E, 1, 2) (E, 2, 4)
(E, 2, 5) -> (E, 2, 4) (E, 3, 5)
(E, 0, 2) -> (E, 0, 1) (E, 1, 2)
(E, 1, 3) -> (E, 1, 2) (E, 2, 3)
(E, 2, 4) -> (E, 2, 3) (E, 3, 4)
(E, 3, 5) -> (E, 3, 4) (E, 4, 5)
(E, 0, 1) -> (F, 0, 1)
           | (F, 0, 1) (E, 1, 1)
(E, 1, 2) -> (F, 1, 2)
           | (F, 1, 2) (E, 2, 2)                
(E, 2, 3) -> (F, 2, 3)
           | (F, 2, 3) (E, 3, 3)
(E, 3, 4) -> (F, 3, 4)
           | (F, 3, 4) (E, 4, 4)
(E, 4, 5) -> (F, 4, 5)
           | (F, 4, 5) (E, 5, 5)
```