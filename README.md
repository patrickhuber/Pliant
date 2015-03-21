# Pliant
Implementation of a modified Earley parser in C# inspired by the Marpa Parser project.

## Description
Pliant is a table driven parser that implements the Earley algorithm. Two optimizations are added to handle issues with the original Earley implementation: 

1. Optimization from joop leo to efficiently handle right recursions. 
2. Bug fix from Aycock and Horspool to handle nullable predictions

## References

* [berkeley earley cs164](http://inst.eecs.berkeley.edu/~cs164/fa10/earley/earley.html)
* [washington edu ling571](http://courses.washington.edu/ling571/ling571_fall_2010/slides/parsing_earley.pdf)
* [unt cse earley](http://www.cse.unt.edu/~tarau/teaching/NLP/Earley%20parser.pdf)
* [wikipedia](http://en.wikipedia.org/wiki/Earley_parser)
* [optimizing right recursion](http://loup-vaillant.fr/tutorials/earley-parsing/right-recursion)
* [mapra parser](http://jeffreykegler.github.io/Ocean-of-Awareness-blog/)
* [joop leo - optimizing right recursions](http://www.sciencedirect.com/science/article/pii/030439759190180A)
* [parsing techniques - a practical guide](http://amzn.com/B0017AMLL8)
* [practical earley parsing](http://webhome.cs.uvic.ca/~nigelh/Publications/PracticalEarleyParsing.pdf)
* [detailed description of leo optimizations and internals of marpa](https://github.com/jeffreykegler/kollos/blob/master/notes/misc/leo2.md)