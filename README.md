# Modélisation Géométrique

## **TP1**

Modélisation géométrique de mesh dynamiquement sur unity.

### Générer un mesh

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

### Tester un mesh

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

### Résultats

#### Rectangle
<div align="center">
    <img src="Images/results/rect.png" alt="Rect" width="400" style="border-radius: 10px;">
</div>

#### Plan
<div align="center">
    <img src="Images/results/plane.png" alt="Plane" width="400" style="border-radius: 10px;">
</div>

#### Cylindre
<div align="center">
    <img src="Images/results/cylinder.png" alt="Cylinder" width="400" style="border-radius: 10px;">
</div>

#### Sphère
<div align="center">
    <img src="Images/results/sphere.png" alt="Sphere" width="400" style="border-radius: 10px;">
</div>

#### Cône
<div align="center">
    <img src="Images/results/cone.png" alt="Cone" width="400" style="border-radius: 10px;">
</div>

#### Tronquage
<div align="center">
    <img src="Images/results/cylinder_trunc.png" alt="Cylinder truncated" width="400" style="border-radius: 10px;">
    <img src="Images/results/sphere_trunc.png" alt="Sphere truncated" width="400" style="border-radius: 10px;">
    <img src="Images/results/cone_trunc.png" alt="Cone truncated" width="400" style="border-radius: 10px;">
</div>

### Scripts

Les scripts pour générer les meshs sont dans *Assets/Scripts/MeshUtils.cs*.


## **TP2**

Ouverture et sauvegarde de fichiers OFF

### OFF File windows

The path to access OFF tools.

<div align="center">
    <img src="Images/off_files/offFiles_path.png" alt="Path to access off tools" width="400" style="border-radius: 10px;">
</div>

#### Create mesh from OFF file

The window to create a mesh from an OFF file.

<div align="center">
    <img src="Images/off_files/createMesh.png" alt="Create mesh from off file window" width="400" style="border-radius: 10px;">
</div>

#### Save mesh into OFF file

The window to save a mesh into an OFF file.

<div align="center">
    <img src="Images/off_files/saveMesh.png" alt="Save mesh into off file window" width="400" style="border-radius: 10px;">
</div>

### Results

#### Mesh import

The results of the import from OFF files.

<div align="center">
    <img src="Images/results/rabbit.png" alt="Rabbit" width="400" style="border-radius: 10px;">
    <img src="Images/results/buddha.png" alt="Buddha" width="200" style="border-radius: 10px;">
</div>

#### Mesh export

*Here I export the buddha mesh without 10 000 of his triangle, and after, that I import this new mesh. This manip shows that the export function is working.*

The result of the success of export the buddha without all his mesh triangles.

<div align="center">
    <img src="Images/results/buddha_trunc.png" alt="Buddha truncated" width="300" style="border-radius: 10px;">
</div>

## **TP3**

### Spatial Enumeration

The object is to create a volumetric representation of a sphere.

### 