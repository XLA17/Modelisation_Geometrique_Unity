# TP1 - Modélisation Géométrique

Modélisation Géométrique de mesh dynamiquement sur unity.

## Générer un mesh

Pour générer un mesh custom, il faut accéder à la fenêtre **Generate Custom Mesh**, accessible par *Generate -> Mesh*.

<div align="center">
    <img src="Images/generate/generate_mesh_path.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>
<div align="center">
    <img src="Images/generate/generate_mesh_window.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

Cette fenêtre pêrmet de créer les différents types de mesh du TP1, ainsi que la possibilité de les tronquer.

<div align="center">
    <img src="Images/generate/generate_mesh_choice.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

Une fois le bouton **Generate** appuyé, le mesh sera créé sous *Assets/Meshs*.

*Il y a actuellement tous les meshs créés pour les exemples qui suivent.* 

<div align="center">
    <img src="Images/generate/assets_mesh.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

## Tester un mesh

Pour tester un mesh nouvellement créé, il suffit d'accéder à une scène, puis de créer un **Empty** gameObject.

*Dans ma scène il existe déjà un objet **TestObject**.*

<div align="center">
    <img src="Images/tests/hierarchy.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

Il faut ajouter à ce gameObject un **Mesh Filter** ainsi qu'un **Mesh Renderer**.

*Je conseille de mettre le matériau **SpatialMappingWireframe** dans le Mesh Renderer afin de bien voir les triangles du mesh.*

<div align="center">
    <img src="Images/tests/inspector.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

Ensuite, il ne reste plus qu'à sélectionner le mesh voulu dans le **Mesh Filter**.

<div align="center">
    <img src="Images/tests/meshs.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

## Résultats

### Rectangle
<div align="center">
    <img src="Images/results/rect.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

### Plan
<div align="center">
    <img src="Images/results/plane.png" alt="Plane" width="400" style="border-radius: 10px;">
</div>

### Cylindre
<div align="center">
    <img src="Images/results/cylinder.png" alt="Cylinder" width="400" style="border-radius: 10px;">
</div>

### Sphère
<div align="center">
    <img src="Images/results/sphere.png" alt="Sphere" width="400" style="border-radius: 10px;">
</div>

### Cône
<div align="center">
    <img src="Images/results/cone.png" alt="Cone" width="400" style="border-radius: 10px;">
</div>

### Tronquage
<div align="center">
    <img src="Images/results/cylinder_trunc.png" alt="Cylinder truncated" width="400" style="border-radius: 10px;">
    <img src="Images/results/sphere_trunc.png" alt="Sphere truncated" width="400" style="border-radius: 10px;">
    <img src="Images/results/cone_trunc.png" alt="Cone truncated" width="400" style="border-radius: 10px;">
</div>

## Scripts

Les scripts pour générer les meshs sont dans *Assets/Scripts/MeshUtils.cs*.