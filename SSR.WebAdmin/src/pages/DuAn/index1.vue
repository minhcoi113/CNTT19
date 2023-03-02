<script>
import Layout from "../../layouts/main";
import appConfig from "@/app.config";
import Multiselect from "vue-multiselect";
import {projectModel} from "@/models/projectModel";
import VueUploadMultipleImage from "vue-upload-multiple-image";
import {required} from "vuelidate/lib/validators";
import urlSlug from 'url-slug'
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {CONSTANTS} from "@/helpers/constants";
import {labelModel} from "@/models/labelModel";
import {userModel} from "@/models/userModel";
import {groupModel} from "@/models/groupModel";
import * as XLSX from 'xlsx-js-style';
export default {
  page: {
    title: "Quản lý dự án",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout, Multiselect,    VueUploadMultipleImage},
  data() {
    return {
      title: "Quản lý dự án",
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "age",
      sortDesc: false,
      model: projectModel.baseJson(),
      itemFilter:{
        code: null,
        name: null,
      },
      items: [
        {
          text: "project",
          href:"/project",
          // active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      
      fields: [
      { key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: {width: '30px', minWidth: '30px'}
        },
        {
          key: "name",
          label: "Tên",
          sortable: true,
        },
        {
          key: "description",
          label: "Mô tả",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '100px', minWidth: '100px'},
        },
        {
          key: "label",
          label: "label",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '120px', minWidth: '120px'},
        },
       
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: {width: '130px', minWidth: '130px'},
        }
      ],
       editorConfig: {
        toolbar: {
          items: [
            'heading', '|',
            'fontfamily', 'fontsize', '|',
            'uploadImage',
            'code', 'codeBlock', '|',
            'alignment', '|',
            'fontColor', 'fontBackgroundColor', '|',
            'bold', 'italic', 'strikethrough', 'underline', 'subscript', 'superscript', '|',
            'link', '|',
            'outdent', 'indent', '|',
            'bulletedList', 'numberedList', 'todoList', '|',
            'insertTable', '|',
            'undo', 'redo'
          ],
          shouldNotGroupWhenFull: false
        },
        removePlugins: ['Title', 'ImageCaption'],
        simpleUpload: {
          uploadUrl: process.env.VUE_APP_API_URL + "files/upload-ckeditor",
          headers: {
            'Authorization': 'optional_token'
          }
        },
      },
      apiUrl: process.env.VUE_APP_API_URL,
      url: `${process.env.VUE_APP_API_URL}files/upload`,
      urlView: `${process.env.VUE_APP_API_URL}files/view/`,
      dropzoneOptions: {
        url: `${process.env.VUE_APP_API_URL}files/upload`,
        thumbnailWidth: 300,
        thumbnailHeight: 160,
        maxFiles: 1,
        maxFilesize: 30,
        headers: {"My-Awesome-Header": "header value"},
        addRemoveLinks: true,
        acceptedFiles: ".pdf",
        dropzoneClassName: "dropzonevue-box"
      }, 
      optionsUser: [],
      optionsGroup: [],
      optionsLabel: [],
      simpleUpload: {
        uploadUrl: process.env.VUE_APP_API_URL + "files/upload-ckeditor",
        headers: {
          'Authorization': 'optional_token'
        }
      },
    };
  },
  async created() {
    this.getUser();
    this.getGroup();
    this.getLabel();
    if(this.$route.params.id){
      this.getById(this.$route.params.id);
    }else{
      this.model = projectModel.baseJson();
    }
    },
  watch:{
    model: {
      handler: function (val, oldVal) {
        if(val.name == null){
          this.model.slug = "";
        }
        else
          this.model.slug = urlSlug(val.name);
      },
      deep: true
    }
  },
  computed:{
     images() {
      if(this.model && this.model.files){
        let imgs = [];
        this.model.files.map((value, index) =>{
          imgs.push({
            id: index,
            fileId: value.fileId,
            path: this.urlView + value.fileId,
            default: 1,
            highlight: 1,
            caption: value.fileName, // Optional
          })
        })
        console.log(imgs);
        return imgs;
      }
      return [];
    } 
  },
  mounted() {
  },
  validations: {
    model: {
    
      name: {required},
      description: {required},
      // member: {required},
      // group: {required},
      label: {required},
      slug: {required},
    
    },
  },
  
  methods: {
    clearSearch(){
      this.itemFilter.code= null;
      this.itemFilter.name= null;
    },
    async fnGetList() {
         this.$refs.tblList?.refresh()
    },
     async uploadImageSuccess(formData, index, fileList) {
      await this.$store.dispatch("fileStore/uploadFile", formData).then((res) => {
        console.log(res)
        if (res.resultCode === 'SUCCESS') {
          var data = res.data;
          if(this.model.files == null)
            this.model.files = [];
          this.model.files = [...this.model.files,  {fileId: data.id, fileName: data.fileName}]
          return;
        }
      });
    }, 
    beforeRemove (index, done, fileList) {
      console.log(fileList);
      var fileId = fileList.find(x => x.id == index);
      var r = confirm("Xóa hình ảnh")
      if (r == true) {
        if(this.model && this.model.files && this.model.files.length > 0 && fileId){
          this.model.files = this.model.files.filter(x => x.fileId != fileId.fileId);
          console.log(this.model.files)
        }
        done();
      } else {
        console.log(1)
      }
    }, 
    async getUser(){
      await this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsUser = res.data;
          return;
        }
        this.optionsUser = [];
      });
    },

    async getGroup(){
      await this.$store.dispatch("groupStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsGroup = res.data;
          return;
        }
        this.optionsGroup = [];
      });
    },
    async getLabel(){
      await this.$store.dispatch("labelStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.optionsLabel = res.data;
          return;
        }
        this.optionsLabel = [];
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("projectStore/delete", this.model.id).then((res) => {
          if (res.resultCode==='SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        });
        if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
        ) {
          // Update model
          await this.$store.dispatch("projectStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.model= projectModel.baseJson();
              this.$refs.tblList.refresh();
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("projectStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList(); 
              this.model= projectModel.baseJson();
              this.showModal = true;
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    async handleUpdate(id) {
      await this.$store.dispatch("projectStore/getById", id).then((res) => {
        this.$router.push("")
        if (res.resultCode==='SUCCESS') {
          this.model = projectModel.toJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },

    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("projectStore/getPagingParams", params)
        return promise.then(resp => {
          let items = resp.data.data
          this.totalRows = resp.data.totalRows
          this.numberOfElement = resp.data.data.length
          this.loading = false
          return items || []
        })
      } finally {
        this.loading = false
      }
    },
    
    base64ToArrayBuffer(base64) {
      var binaryString = window.atob(base64);
      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
      }
      return bytes;
    },
    handleNewPost(){
      this.$router.push("/tao-project")
    },
    Export(){
      import("../../state/modules/Export2Excel").then(excel=>{
        this.$store.dispatch("projectStore/get").then(res=>{
          console.log(res.data)
          const OBJ=res.data;
          const Header=["Tên","Mô tả","label"];
          const Field=["name","description"];
          const Data=this.FormatJSon(Field,OBJ);
          excel.export_json_to_excel({
            header:Header,
            data:Data,
            sheetName:"Báo cáo",
            filename:"Bao Cao",
            autoWidth:true,
            bookType:"xlsx",
          })
        })
      })
    },
    FormatJSon(FilterData, JsonData){
      return JsonData.map((v)=>FilterData.map((j=>{
        return v[j];
      })))
    }
  },
};
</script>

<template>
  <Layout>
<!--    <PageHeader/>-->
    <div class="row">
    <div class="col-12">
      <div class="card mb-2">
        <div class="card-body">
          <div class="row">
            <div class="col-md-4 col-12 d-flex align-items-center">
              <h4 class="font-size-18 fw-bold cs-title-page">Project</h4>
            </div>
            <div class="col-md-8 col-12 text-end">
              <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
              <b-button
                    variant="primary"
                    type="button"
                    class="btn w-md btn-primary"
                    @click="handleNewPost"
                    size="sm"
                >
                  <i class="mdi mdi-plus me-1"></i> Thêm dự án
                </b-button>
                <b-button
                    variant="primary"
                    type="button"
                    class="btn w-md btn-secondary"
                    @click="Export"
                    size="sm"
                >
                  <i class="fas fa-file-excel"></i> Xuất Excel
                </b-button>
            </div>
          </div>
          <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Nội dung</label>
                      <input
                          v-model = "filter"
                          type="text"
                          class="form-control"
                          placeholder="Tìm kiếm ..."
                      />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="handleSearch" variant="light"
                                    class="btn w-md btn-primary me-2" size="md">
                            <i class="fas fa-search align-middle me-2"></i>
                            Tìm kiếm
                          </b-button>
                        </div>
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light"
                                    class="btn w-md btn-secondary me-2" size="md">
                            <i class="fas fa-redo-alt align-middle me-2"></i>
                            Làm mới
                          </b-button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </b-collapse>
        </div>
      </div>
    </div>
  </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal
                      v-model="showModal"
                      title="Thông tin group"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer"
                    >
                    <div class="row">
                <div class="col-md-9">
                  <div class="row">
                    <div class="col-lg-12 col-md-12 col-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Name</label>
                                                        <span
                                                          class="text-danger">*</span>
                        <input
                            id="validationCustom01"
                            v-model="model.name"
                            type="text"
                            class="form-control"
                            placeholder=""
                            :class="{'is-invalid': submitted && $v.model.name.$error,}"
                        />
                        <div
                            v-if="submitted && !$v.model.name.required"
                            class="invalid-feedback"
                        >
                          Tiêu đề không được để trống.
                        </div>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01">Mô tả</label>
                        <span class="text-danger">*</span>
                        <textarea
                            id="validationCustom01"
                            v-model="model.description"
                            type="text"
                            class="form-control"
                            placeholder=""
                            :class="{'is-invalid': submitted && $v.model.description.$error,}">
                          </textarea>
                            <div v-if="submitted && !$v.model.description.required" class="invalid-feedback">
                              Nội dung không được để trống.
                            </div>
                      </div>
                    </div>
                    
                    <div class="col-lg-12 col-md-12 col-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Slug</label>
                        <span
                            class="text-danger">*</span>
                        <input
                            id="validationCustom01"
                            v-model="model.slug"
                            type="text"
                            class="form-control"
                            placeholder=""
                            :class="{'is-invalid': submitted && $v.model.slug.$error,}"
                        />
                        <div
                            v-if="submitted && !$v.model.slug.required"
                            class="invalid-feedback"
                        >
                          Slug không được để trống.
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                 <div class="col-md-3">
                  <div class="row">
                     <div class="col-md-12 mb-2">
                      <label class="form-label cs-title-form" for="validationCustom01"> Hình ảnh</label>
                      <div class="col-md-12 d-flex justify-content-center" id="my-strictly-unique-vue-upload-multiple-image">
                        <vue-upload-multiple-image
                            @upload-success="uploadImageSuccess"
                            @before-remove="beforeRemove"
                            :data-images="images"
                            idUpload="myIdUpload"
                            editUpload="myIdEdit"
                            :showEdit="false"
                            class="cs-upload-image"
                        ></vue-upload-multiple-image>
                      </div>
                    </div> 
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Group</label>
                        <multiselect
                            v-model="model.group"
                            :options="optionsGroup"
                            :multiple="true"
                            track-by="id"
                            label="name"
                            placeholder="Chọn nhóm"
                            deselect-label="Nhấn để xoá"
                            selectLabel="Nhấn enter để chọn"
                            selectedLabel="Đã chọn"
                            :class="{'is-invalid': submitted && $v.model.group.$error,}"
                        ></multiselect>
                        <div
                            v-if="submitted && !$v.model.group.required"
                            class="invalid-feedback"
                        >
                          Group không được để trống.
                        </div>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01"> Thành viên</label>
                        <multiselect
                            v-model="model.member"
                            :options="optionsUser"
                            :multiple="true"
                            track-by="id"
                            label="fullName"
                            placeholder="Chọn thể loại"
                            deselect-label="Nhấn để xoá"
                            selectLabel="Nhấn enter để chọn"
                            selectedLabel="Đã chọn"
                            :class="{'is-invalid': submitted && $v.model.member.$error,}"
                        ></multiselect>
                        <div
                            v-if="submitted && !$v.model.member.required"
                            class="invalid-feedback"
                        >
                          Thành viên không được để trống.
                        </div>
                      </div>
                    </div>
                    <div class="col-md-12">
                      <div class="mb-2">
                        <label class="form-label cs-title-form" for="validationCustom01">Nhãn</label>
                        <multiselect
                            v-model="model.label"
                            :options="optionsLabel"
                            track-by="id"
                            label="name"
                            placeholder="Chọn thẻ"
                            deselect-label="Nhấn để xoá"
                            selectLabel="Nhấn enter để chọn"
                            selectedLabel="Đã chọn"
                            :multiple="true"
                            :class="{'is-invalid': submitted && $v.model.label.$error,}"
                        ></multiselect>
                        <div
                            v-if="submitted && !$v.model.label.required"
                            class="invalid-feedback"
                        >
                          Nhãn không được để trống.
                        </div>
                      </div>
                    </div>    
                  </div>
                </div>
              </div>
                      <div class="text-end pt-2">
                        <b-button variant="light" class="w-md" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button  type="submit" variant="primary" class="ms-1 w-md">Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <div id="tickets-table_length" class="dataTables_length">
                      <label class="d-inline-flex align-items-center">
                        Hiện
                        <b-form-select
                            class="form-select form-select-sm"
                            v-model="perPage"
                            size="sm"
                            :options="pageOptions"
                        ></b-form-select
                        >&nbsp;dòng
                      </label>
                    </div>
                  </div>
                </div>
                <div class="table-responsive mb-0">
                    <b-table
                      class="datatables custom-table"
                      :items="myProvider"
                      :fields="fields"
                      responsive="sm"
                      :per-page="perPage"
                      :current-page="currentPage"
                      :sort-by.sync="sortBy"
                      :sort-desc.sync="sortDesc"
                      :filter="filter"
                      :filter-included-fields="filterOn"
                      ref="tblList"
                      primary-key="id"
                      :busy.sync="isBusy"
                      tbody-tr-class="b-table-chucvu"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    
                    <template v-slot:cell(label)="data">
                      <div v-for="(value , index) in data.item.label" :key="index">
                        <span  class="badge bg-success ms-1"> {{value.name}}</span>
                      </div>
                    </template>
                    
                    <template v-slot:cell(process)="data">
                       <router-link :to='`/du-an/chi-tiet/${data.item.id}`'>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          >
                        <i class="fas fa-pencil-alt"></i>
                      </button>

                    </router-link>
                      <button
                      
                          type="button"
                          size="sm"
                          class="btn btn-delete btn-sm"
                          v-on:click="handleShowDeleteModal(data.item.id);">
                        <i class="fas fa-trash-alt"></i>
                      </button>
                    </template>
                  </b-table>
                  <template v-if="isBusy">
                    <div align="center">Đang tải dữ liệu</div>
                  </template>
                  <template v-if="totalRows <= 0 && !isBusy">
                    <div align="center">Không có dữ liệu</div>
                  </template>
                </div>
                <div class="row">
                  <b-col>
                    <div>Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <!-- pagination -->
                        <b-pagination
                            class="pagination-rounded"
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                            size="sm"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>

              </div>
            </div>
            <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit
                        class="btn btn-outline-info m-1"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit
                        class="btn btn-danger btn m-1"
                        v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style >
.td-stt {
  text-align: center;
  width: 55px;
}
.td-username {
  text-align: center;
  width: 120px;
}
.td-ten {
  text-align: center;
  width: 140px;
}
.td-email {
  text-align: center;
  width: 120px;
}
.td-donVi{
  text-align: center;
  width: 180px;
}
.td-xuly {
  text-align: center;
  width: 80px;
}
.td-sodienthoai {
  text-align: center;
  width: 100px;
}

</style>
<style lang="scss">
#my-strictly-unique-vue-upload-multiple-image {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>