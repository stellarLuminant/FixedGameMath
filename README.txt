# FixedGameMath
###### Fork of [asik/FixedMath.Net](https://github.com/asik/FixedMath.Net).

A fixed-point numerics library that implements Fix64, a Q31.32 signed number alongside basic functions. This fork takes a more opinionated approach towards usage with a 2D game framework such as MonoGame.

#### Tests on master: [![Build Status](https://travis-ci.com/stellarLuminant/FixedMath.NET.svg?branch=master)](https://travis-ci.com/stellarLuminant/FixedMath.NET)

### Roadmap

- **Convert trigonometric functions to degrees:** Degrees are significantly advantageous in fixed-point arithmetic, allowing us to represent special values such as PI and PI/2 as fully precise constants while also offering about 9 more bits of precision in all other operations.
- **Add full-range precision parsing and printing:** Fix64 currently offers no native parsing solution, and casts to decimal in order to print.
- **Add basic geometric structs:** Reimplementations of common structs such as Rects or Vector2 would mean less work on the user to integrate this library.
- **Integrate test coverage analysis:** It would help to ensure all code paths are well-tested.
- **Implement a determinism test suite:** Current test suite verifies functions by comparing them to verified implementations (such as System.Math floating point) with a small range of tolerance for error. A determinism test suite would instead store the outputs from a single target platform and compare them for bit-exactness on other platforms.
