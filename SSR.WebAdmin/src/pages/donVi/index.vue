<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {menuModel} from "@/models/menuModel";
import Treeselect from "@riophae/vue-treeselect";
import {linhVucModel} from "@/models/linhVucModel";
import {donViModel} from "@/models/donViModel";

export default {
  page: {
    title: "Quản lý đơn vị",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout, Treeselect},
  data() {
    return {
      // data: [],
      title: "Quản lý đơn vị",
      items: [
        {
          text: "Đơn vị",
          active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      showModal: false,
      showDeleteModal: false,
      submitted: false,
      model: donViModel.baseJson(),
      listParent: [],
      treeView: [],
      itemEvents: {
        mouseover: function () {

        },
        contextmenu: function () {
          arguments[2].preventDefault()
        }
      },
      itemFilter:{
        code: null,
        name: null,
      }
    };
  },
  validations: {
    model: {
      id: {required},
      name: {required},
    
    },
  },
  created() {
    this.GetTreeList();
  },
  methods: {
    clearSearch(){
      this.itemFilter.code= null;
      this.itemFilter.name= null;
    },
    addDonViToModel(node, instanceId) {
      if (node.id) {
        this.model.parentId = node.id;
      }
    },
    normalizer(node) {
      if (node.children == null || node.children == 'null') {
        delete node.children;
      }
    },
    async GetTreeList() {
      await this.$store.dispatch("donViStore/getTree").then((res) => {
        this.treeView = res.data;
        console.log("log tree", this.treeView)
      })
    },
    async itemClick(node) {
      this.handleDetail(node.model.id)
    },
    async handleDetail(id) {
      await this.$store.dispatch("donViStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = donViModel.fromJson(res.data);
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
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
          await this.$store.dispatch("donViStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.GetTreeList();
              this.model = donViModel.baseJson()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("donViStore/create", donViModel.toJson(this.model)).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.GetTreeList();
              this.model = donViModel.baseJson()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("donViStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.showDeleteModal = false;
            this.GetTreeList();
            this.model = donViModel.baseJson();
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          // });
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    handleResetForm() {
      this.model = menuModel.baseJson()
    },
  },
  computed: {},
  watch: {
    showModal(status) {
      if (status == false) this.model = menuModel.baseJson();
    },
    model() {
      return this.model;
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    }
  },
};
</script>

<template>
  <Layout>
<!--    <PageHeader :title="title" :items="items"/>-->
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold text-dark">Quản lý đơn vị</h4>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="row flex-md-row flex-column-reverse">
      <div class="col-md-6 col-12">
        <div class="card">
          <div class="card-body">
            <v-jstree :data="treeView" text-field-name="label" @item-click="itemClick"
                      :item-events="itemEvents"></v-jstree>
          </div>
        </div>
      </div>
      <div class="col-md-6 col-12">
        <div class="card">
          <div class="card-body">
            <form @submit.prevent="handleSubmit"
                  ref="formContainer"
            >
              <div class="row">
                <div class="col-12">
                  <input type="hidden" v-model="model.id"/>
                  <div class="mb-3">
                    <label class="text-left">Mã đơn vị</label>
                    <span style="color: red">&nbsp;*</span>
                    <input
                        id="id"
                        v-model="model.id"
                        type="text"
                        class="form-control"
                        placeholder="Nhập mã đơn vị"
                        :class="{
                                                      'is-invalid':
                                                        submitted && $v.model.id.$error,
                                                    }"
                    />
                    <div
                        v-if="submitted && !$v.model.id.required"
                        class="invalid-feedback"
                    >
                      Mã đơn vị không được trống.
                    </div>
                  </div>
                  <div class="mb-3">
                    <label class="text-left">Tên đơn vị</label>
                    <span style="color: red">&nbsp;*</span>
                    <input
                        id="path"
                        v-model="model.name"
                        type="text"
                        class="form-control"
                        placeholder="Nhập tên đơn vị"
                        :class="{
                                                      'is-invalid':
                                                        submitted && $v.model.name.$error,
                                                    }"
                    />
                    <div
                        v-if="submitted && !$v.model.name.required"
                        class="invalid-feedback"
                    >
                      Tên đơn vị không được trống.
                    </div>
                  </div>
                  <div class="mb-3">
                    <label class="text-left">Cấp đơn vị</label>
                    <span style="color: red">&nbsp;*</span>
                    <input
                        id="icon"
                        v-model="model.capDV"
                        type="number"
                        class="form-control"
                        placeholder="Nhập cấp đơn vị"
                        :class="{
                                                      'is-invalid':
                                                        submitted && $v.model.capDV.$error,
                                                    }"
                    />
                    <div
                        v-if="submitted && !$v.model.capDV.required"
                        class="invalid-feedback"
                    >
                      Cấp đơn vị không được trống.
                    </div>
                  </div>
                  <div class="mb-3">
                    <label class="text-left">Đơn vị cha</label>
                    <treeselect
                        v-on:select="addDonViToModel"
                        :normalizer="normalizer"
                        :options="treeView"
                        :value="model.parentId"
                        :searchable="true"
                        :show-count="true"
                        :default-expand-level="1"
                        placeholder="Chọn đơn vị cha"
                    >
                      <label slot="option-label"
                             slot-scope="{ node, shouldShowCount, count, labelClassName, countClassName }"
                             :class="labelClassName">
                        {{ node.label }}
                        <span v-if="shouldShowCount" :class="countClassName">({{ count }})</span>
                      </label>
                    </treeselect>
                  </div>

                </div>
              </div>
              <div class="text-end pt-2 mt-3">
                <b-button size="sm" v-if="model.id" type="button" variant="warning" class="ms-1 w-md"
                          v-on:click="handleResetForm"
                > Đặt lại
                </b-button>
                <b-button size="sm" v-if="model.id" type="button" variant="danger" class="ms-1 w-md"
                          v-on:click="handleShowDeleteModal(model.id)"
                > Xóa
                </b-button>
                <b-button type="submit" size="sm" variant="primary" class="ms-1 w-md"
                >Lưu
                </b-button>
              </div>
            </form>
          </div>
          <b-modal
              v-model="showDeleteModal"
              centered
              title="Xóa dữ liệu"
              title-class="font-18"
              no-close-on-backdrop
          >
            <p>
              Dữ liệu xóa sẽ không được phục hồi !
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
  </Layout>
</template>
