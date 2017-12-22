# Ask from the client

## the question
I want to add an information about the weather on my photos

## The solutions

### Technical
To apply the modification asked by the client, we must do a list of changes: 

1. Add a private string attribute in the ImageMetadata class in the file class/C_ImageMetadata.cs
2. Add Getter and Setter for this attribute in the ImageMetadata class.
3. Add a public boolean function with the name "setMetadataWeather()".
4. Add a public string function with the name "getMetadataWeather()".
5. Add the attribute in the constructor.
6. Add the attribute in the saveMetadata function.
7. Add a text box on the layout (winMain_V2.xaml).
8. Add a LostFocus event on the text box.
9. In this event set the metadata weather with the content of the text box.
10. Add the weather metadata in the "checkFilterAndSearch" function.

But to apply this modifications, we encoured a problem because the ".jpg" files doesn't have a metadata for weather, so we need to take an other and use it to insert weather in. We think we will use the subject metadata for use weather informations but it will use this space so if we will add subject value later on the project we can't do it or we need to change the weather informations metadata place.

### For the client
At the moment the client ask us to add information about the weather, we need to open again the project and recreate a planning, logbook and specification with the modifications we need to do.
The better way is to have a meeting with the client and ask if he need only this modification or other too and define a deadline for this modifications.
After, we can organise the team for this project and apply the technical modifications.
