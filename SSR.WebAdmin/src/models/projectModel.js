
const baseJson = () => {
    return {
        id: null,
        name: null,
        description: null,
        slug: null,
        group: null,
        label: null,
        member: null,
        files: null,
    }
}
const toJson = (item) => {
  return {
      id: item.id,
      name: item.name,
      description: item.description,
      slug: item.slug,
      member: item.member,
      group: item.group,
      label: item.label,
      files: item.files
  }
}
const fromJson = (item) => {
  return {
      id: item.id,
      name: item.name,
      description: item.description,
      slug: item.slug,
      member: item.member,
      group: item.group,
      label: item.label,
      files: item.files     
  }
}
const toListGroup = (items) =>{
  if(items.length > 0){
      let data = [];
      items.map((value, index) =>{
          data.push(fromJson(value));
      })
      return data??[];
  }
  return [];
}

export const projectModel = {
  baseJson, toListGroup, fromJson, toJson
}
