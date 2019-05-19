For the purposes of this Excercise I am making use of EF Cores Seeding mechanism in the context.

I have made the assumption that for the Top 5 endpoint the you will not require the additional filtering as detailed in the spec for endpoint A. 
if that assumption is incorrect, I would have refactored the filter code so I can call it code all it when defining the top 5 service method instead of using 
movieContext.Movies .

As for tests, I am only testing and asserting on the things that are being tested.

Given more time, would like to have made builders for Test Entities