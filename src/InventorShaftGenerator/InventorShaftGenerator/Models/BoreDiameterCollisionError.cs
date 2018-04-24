using System;
using System.ComponentModel;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models
{
    public class BoreDiameterCollisionError : NotifyPropertyChanged
    {
        private float maxDiameter;
        private ShaftSection collisedToSection;
        private ShaftSection linkedBoreSection;

        public BoreDiameterCollisionError(ShaftSection linkedBore, ShaftSection collisedTo)
        {
            this.LinkedBore = linkedBore;
            this.CollisedToSection = collisedTo;

            switch (this.collisedToSection)
            {
                case CylinderSection cylinderSection:
                    this.MaxDiameter = cylinderSection.Diameter - 0.1f;
                    break;

                case ConeSection coneSection:
                    this.MaxDiameter = Math.Min(coneSection.Diameter1, coneSection.Diameter2) - 0.1f;
                    break;

                case PolygonSection polygonSection:
                    this.MaxDiameter = polygonSection.InscribedCircleDiameter - 0.1f;
                    break;
            }

            this.CollisedToSection.PropertyChanged += OnDiameterChanged;
            this.LinkedBore.PropertyChanged += OnDiameterChanged;
        }

        private void OnDiameterChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(CylinderSection.Diameter) &&
                args.PropertyName != nameof(ConeSection.Diameter1) &&
                args.PropertyName != nameof(ConeSection.Diameter2) &&
                args.PropertyName != nameof(PolygonSection.InscribedCircleDiameter))
            {
                return;
            }

            switch (this.CollisedToSection)
            {
                case CylinderSection cylinderSection:
                {
                    if ((cylinderSection.Diameter + 0.1) > (this.LinkedBore is ConeSection coneBore
                            ? Math.Min(coneBore.Diameter1,
                                coneBore.Diameter2)
                            : ((CylinderSection) this.linkedBoreSection).Diameter))
                    {
                        this.linkedBoreSection.BoreDiameterCollisionError = null;
                    }
                    else
                    {
                        this.MaxDiameter = cylinderSection.Diameter - 0.1f;
                    }

                    break;
                }

                case ConeSection coneSection:
                {
                    if ((Math.Min(coneSection.Diameter1, coneSection.Diameter2) + 0.1) >
                        (this.LinkedBore is ConeSection coneBore
                            ? Math.Min(coneBore.Diameter1,
                                coneBore.Diameter2)
                            : ((CylinderSection) this.linkedBoreSection).Diameter))
                    {
                        this.linkedBoreSection.BoreDiameterCollisionError = null;
                    }
                    else
                    {
                        this.MaxDiameter = Math.Min(coneSection.Diameter1, coneSection.Diameter2) - 0.1f;
                    }

                    break;
                }

                case PolygonSection polygonSection:
                {
                    if ((polygonSection.InscribedCircleDiameter + 0.1) >
                        (this.LinkedBore is ConeSection coneBore
                            ? Math.Min(coneBore.Diameter1,
                                coneBore.Diameter2)
                            : ((CylinderSection) this.linkedBoreSection).Diameter))
                    {
                        this.linkedBoreSection.BoreDiameterCollisionError = null;
                    }
                    else
                    {
                        this.MaxDiameter = polygonSection.InscribedCircleDiameter - 0.1f;
                    }

                    break;
                }
            }
        }

        public float MaxDiameter
        {
            get => this.maxDiameter;
            set => SetProperty(ref this.maxDiameter, value);
        }

        public ShaftSection LinkedBore
        {
            get => this.linkedBoreSection;
            set => SetProperty(ref this.linkedBoreSection, value);
        }

        public ShaftSection CollisedToSection
        {
            get => this.collisedToSection;
            set => SetProperty(ref this.collisedToSection, value);
        }
    }
}