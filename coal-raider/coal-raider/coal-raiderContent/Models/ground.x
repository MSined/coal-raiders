xof 0303txt 0064
#Exported from Wings 3D 1.4.1
		Material cube2_auv {
			1.0; 1.0; 1.0; 1.0;;
		1.0;
			1.0; 1.0; 1.0;;
			0.0; 0.0; 0.0;;
			TextureFilename { "../Textures/groundTexture.png"; }
		}
		Material default {
			0.7898538076923077; 0.8133333333333334; 0.6940444444444445; 1.0;;
		1.0;
			1.0; 1.0; 1.0;;
			0.0; 0.0; 0.0;;
		}
Frame cube2 {
	Mesh {
		6;
		-1.0, 0.0040000000000000036, 1.0;,
		1.0, 0.0040000000000000036, 1.0;,
		1.0, 0.0040000000000000036, -1.0;,
		-1.0, 0.0040000000000000036, 1.0;,
		1.0, 0.0040000000000000036, -1.0;,
		-1.0, 0.0040000000000000036, -1.0;;

		2;
		3;0, 1, 2 ;,
		3;3, 4, 5 ;;

		MeshNormals {
			6;		0.0, 1.0, 0.0;,
		0.0, 1.0, 0.0;,
		0.0, 1.0, 0.0;,
		0.0, 1.0, 0.0;,
		0.0, 1.0, 0.0;,
		0.0, 1.0, 0.0;;
		2;
		3;0, 1, 2 ;,
		3;3, 4, 5 ;;
		}
		MeshTextureCoords {
		6;
		1.1102230246251565e-16, 1.0;,
		0.9999999999999999, 0.9999999999999998;,
		0.9999999999999998, 0.0;,
		1.1102230246251565e-16, 1.0;,
		0.9999999999999998, 0.0;,
		0.0, 1.1102230246251565e-16;;
		}
		MeshMaterialList {
			2;
			2;
			0;
			0;
			;
			{cube2_auv}
			{default}
		}
	}}