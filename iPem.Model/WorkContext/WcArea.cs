using iPem.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iPem.Model {
    [Serializable]
    public partial class WcArea {
        private readonly Lazy<HashSet<string>> _keys;
        private readonly Lazy<List<WcArea>> _roots;

        public WcArea(Area current) {
            this.Current = current;

            //延迟加载属性
            this._keys = new Lazy<HashSet<string>>(() => {
                var __keys = new HashSet<string>();
                __keys.Add(this.Current.Id);
                foreach (var child in this.Children) {
                    __keys.Add(child.Current.Id);
                }

                return __keys;
            });
            this._roots = new Lazy<List<WcArea>>(() => {
                if (!this.HasChildren) return new List<WcArea>();
                return this.Children.FindAll(c => c.Current.ParentId == this.Current.Id);
            });
        }

        public Area Current { get; private set; }

        public List<WcArea> Parents { get; private set; }

        public List<WcArea> Children { get; private set; }

        public HashSet<string> Keys {
            get { return this._keys.Value; }
        }

        public List<WcArea> ChildRoot {
            get { return this._roots.Value; }
        }

        public bool HasParents {
            get { return (this.Parents.Count > 0); }
        }

        public bool HasChildren {
            get { return (this.Children.Count > 0); }
        }

        public virtual void Initializer(List<WcArea> entities) {
            this.Parents = new List<WcArea>();
            this.Children = new List<WcArea>();
            this.SetAreaParents(entities, this, this);
            this.SetAreaChildren(entities, this, this);
        }

        public virtual string[] ToPath() {
            var paths = new List<string>();
            if(this.HasParents)
                paths.AddRange(this.Parents.Select(p => p.Current.Id));

            if(this.Current != null)
                paths.Add(this.Current.Id);

            return paths.ToArray();
        }

        public override string ToString() {
            if(this.Current == null)
                return null;

            if(!this.HasParents)
                return this.Current.Name;

            return string.Format("{0},{1}", string.Join(",", this.Parents.Select(p => p.Current.Name)), this.Current.Name);
        }

        private void SetAreaParents(List<WcArea> source, WcArea target, WcArea current) {
            var parent = source.Find(a => a.Current.Id == current.Current.ParentId);
            if(parent != null) {
                SetAreaParents(source, target, parent);
                target.Parents.Add(parent);
            }
        }

        private void SetAreaChildren(List<WcArea> source, WcArea target, WcArea current) {
            var children = source.FindAll(a => a.Current.ParentId == current.Current.Id);
            if(children.Count > 0) {
                target.Children.AddRange(children);
                foreach(var child in children) {
                    SetAreaChildren(source, target, child);
                }
            }
        }
    }
}
