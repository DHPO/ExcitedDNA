using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideBuilder : AbstractBuilder {
	public Nucleotide.Type type;

	protected override void Build() {
		GameObject n = Instantiate(prefab, this.transform.position, this.transform.rotation) as GameObject;
		n.GetComponent<Nucleotide>().setType(this.type);
	}
}
