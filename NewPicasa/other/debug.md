# Corrections bug

## (Corrected) Erreur au clique sur une image si elle ne contient pas d’auteur ->
Soit on part du principe que le prof aura toujours des images avec un auteur ou alors on à autre chose à faire mais sinon... (vla l'erreur)

`` authors = mdtFile.Author.ToArray(); ``

## (Dead) Impossible de changer le nom de l'image sans aller dans "Rename"
Tout est dit

## (Corrected) rajouter un « ; » pour créer un tag supplémentaire
On est obligé de mettre un ";" pour mettre plusieurs tag dans les métas, peux être l'indiquer ?

## (Corrected) Comment on lance la recherche ? (pas trouvé)

## (Corrected) Bug responsive (je vais le faire normalement)

## (Corrected) Btn sauvegarder n'indique pas si sauvé ou Erreur
"Mais si y a une erreur VS va le signaler blablabla" TA GUEULE l'utilisateur ne sait pas ce qu'il s'est passé après cette action, il te ferra un grand câlin si il apprend que ça a marché (Et encore mieux si le temps le permet, afficher l'élément sauvegardé genre "Vos tags on été modifiés")

## (Corrected) Btn sauvegarder indique une erreur GG
La comme ça on a une erreur, au moment ou tu cliques sur sauvegarder sur une image du prof, voici le resultat ->  `List<string> list = this._tags.ToList<string>();``

## (Pas réussi à reproduire) Ne peut pas importer d’image ->
``FileStream fleFile = File.Open(filesDirectory[count],FileMode.Open);``
